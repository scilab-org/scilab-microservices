using Marten;
using Management.Application.Dtos.Dashboard;
using Management.Application.Services;
using Management.Domain.Entities;
using Management.Domain.Enums;

namespace Management.Application.Features.Dashboard;

#region Result types

[ExcludeFromCodeCoverage]
public sealed class ProjectStatusCount
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class AdminDashboardKpis
{
    public ProjectKpis Projects { get; set; } = new();
    public SubmissionStatusKpis SubmissionStatus { get; set; } = new();
    public PaperBankKpis PaperBank { get; set; } = new();
    public JournalKpis Journals { get; set; } = new();
    public TemplateKpis Templates { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public sealed class ProjectKpis
{
    public long Total { get; set; }
    public Dictionary<string, long> ByStatus { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class SubmissionStatusKpis
{
    public Dictionary<string, int> Counts { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class PaperBankKpis
{
    public long Total { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class JournalKpis
{
    public long Total { get; set; }
    public long JournalCount { get; set; }
    public long ConferenceCount { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class TemplateKpis
{
    public long Total { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class RecentProjectItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class AdminDashboardResult
{
    public string Role { get; set; } = "admin";
    public AdminDashboardKpis Kpis { get; set; } = new();
    public List<RecentProjectItem> RecentProjects { get; set; } = [];
    public List<LabRecentPaperDto> RecentPapers { get; set; } = [];
}

#endregion

public record GetAdminDashboardQuery : IQuery<AdminDashboardResult>;

public sealed class GetAdminDashboardQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService,
    IRedisService redisService)
    : IQueryHandler<GetAdminDashboardQuery, AdminDashboardResult>
{
    private const string CacheKey = "dashboard:admin:kpis";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<AdminDashboardResult> Handle(
        GetAdminDashboardQuery request,
        CancellationToken cancellationToken)
    {
        // Always fetch Lab data — needed for recentPapers (not cached) and as input to KPI cache factory
        var labData = await labApiService.GetAdminDashboardKpisAsync(cancellationToken);

        var kpis = await redisService.GetOrSetCacheAsync<AdminDashboardKpis>(
            CacheKey,
            ct => BuildKpisAsync(labData, ct),
            CacheTtl,
            cancellationToken);

        var recentProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == null)
            .OrderByDescending(x => x.CreatedOnUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new AdminDashboardResult
        {
            Kpis = kpis ?? new AdminDashboardKpis(),
            RecentProjects = recentProjects.Select(p => new RecentProjectItem
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                Status = (int)p.Status,
                CreatedAt = p.CreatedOnUtc
            }).ToList(),
            RecentPapers = labData.RecentPapers
        };
    }

    private async Task<AdminDashboardKpis> BuildKpisAsync(LabAdminDashboardKpisDto labData, CancellationToken ct)
    {
        var allProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == null)
            .ToListAsync(ct);

        var projectByStatus = allProjects
            .GroupBy(x => x.Status)
            .ToDictionary(g => g.Key.ToString().ToLowerInvariant(), g => (long)g.Count());

        var totalProjects = (long)allProjects.Count;

        var submissionCounts = labData.SubmissionStatusCounts
            .ToDictionary(x => MapSubmissionStatus(x.Status), x => x.Count);

        return new AdminDashboardKpis
        {
            Projects = new ProjectKpis
            {
                Total = totalProjects,
                ByStatus = projectByStatus
            },
            SubmissionStatus = new SubmissionStatusKpis
            {
                Counts = submissionCounts
            },
            PaperBank = new PaperBankKpis
            {
                Total = labData.PaperBankTotal
            },
            Journals = new JournalKpis
            {
                Total = labData.JournalTotal + labData.ConferenceTotal,
                JournalCount = labData.JournalTotal,
                ConferenceCount = labData.ConferenceTotal
            },
            Templates = new TemplateKpis
            {
                Total = labData.TemplateTotal
            }
        };
    }

    private static string MapSubmissionStatus(int status) => status switch
    {
        1 => "draft",
        2 => "submitted",
        3 => "revisionRequired",
        4 => "resubmitted",
        5 => "accepted",
        6 => "published",
        7 => "rejected",
        8 => "onHold",
        _ => status.ToString()
    };
}

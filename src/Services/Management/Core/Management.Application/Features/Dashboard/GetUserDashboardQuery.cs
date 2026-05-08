using Marten;
using Management.Application.Dtos.Dashboard;
using Management.Application.Services;
using Management.Domain.Entities;
using Management.Domain.Enums;

namespace Management.Application.Features.Dashboard;

#region Result types

[ExcludeFromCodeCoverage]
public sealed class UserMyProjectsKpis
{
    public long Total { get; set; }
    public long Active { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class UserMyTasksKpis
{
    public long Total { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class UserMyPapersKpis
{
    public long Total { get; set; }
    public Dictionary<string, int> BySubmissionStatus { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class UserDashboardKpis
{
    public UserMyProjectsKpis MyProjects { get; set; } = new();
    public UserMyTasksKpis MyTasks { get; set; } = new();
    public UserMyPapersKpis MyPapers { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public sealed class UserDashboardResult
{
    public string Role { get; set; } = "user";
    public UserDashboardKpis Kpis { get; set; } = new();
    public List<LabRecentTaskDto> MyRecentTasks { get; set; } = [];
    public List<LabUserRecentPaperDto> MyRecentPapers { get; set; } = [];
}

#endregion

public record GetUserDashboardQuery(Guid UserId, string Username) : IQuery<UserDashboardResult>;

public sealed class GetUserDashboardQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService,
    IRedisService redisService)
    : IQueryHandler<GetUserDashboardQuery, UserDashboardResult>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    public async Task<UserDashboardResult> Handle(
        GetUserDashboardQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Load the user's member records to get projectIds and memberIds
        var members = await session.Query<MemberEntity>()
            .Where(x => x.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        var memberIds = members.Select(m => m.Id).ToArray();
        var projectIds = members.Select(m => m.ProjectId).Distinct().ToList();

        // 2. Count active top-level projects the user belongs to
        long totalProjects = projectIds.Count;
        long activeProjects = 0;
        if (projectIds.Count > 0)
        {
            activeProjects = await session.Query<ProjectEntity>()
                .Where(x => projectIds.Contains(x.Id)
                             && x.ParentProjectId == null
                             && x.Status == ProjectStatus.Active)
                .CountAsync(cancellationToken);

            totalProjects = await session.Query<ProjectEntity>()
                .Where(x => projectIds.Contains(x.Id) && x.ParentProjectId == null)
                .CountAsync(cancellationToken);
        }

        // 3. Fetch Lab KPIs (tasks + papers); cache the KPI block per user
        var cacheKey = $"dashboard:user:{request.UserId}:kpis";
        var labData = await labApiService.GetUserDashboardKpisAsync(request.Username, memberIds, cancellationToken);

        var kpis = await redisService.GetOrSetCacheAsync<UserDashboardKpis>(
            cacheKey,
            _ => Task.FromResult(BuildKpis(totalProjects, activeProjects, labData)),
            CacheTtl,
            cancellationToken);

        return new UserDashboardResult
        {
            Kpis = kpis ?? BuildKpis(totalProjects, activeProjects, labData),
            MyRecentTasks = labData.RecentTasks,
            MyRecentPapers = labData.RecentPapers
        };
    }

    private static UserDashboardKpis BuildKpis(long totalProjects, long activeProjects, LabUserDashboardKpisDto labData)
    {
        var taskByStatus = labData.TaskStatusCounts
            .ToDictionary(x => MapTaskStatus(x.Status), x => x.Count);

        var paperBySubmission = labData.PaperSubmissionStatusCounts
            .ToDictionary(x => MapSubmissionStatus(x.Status), x => x.Count);

        return new UserDashboardKpis
        {
            MyProjects = new UserMyProjectsKpis
            {
                Total = totalProjects,
                Active = activeProjects
            },
            MyTasks = new UserMyTasksKpis
            {
                Total = labData.TotalTasks,
                ByStatus = taskByStatus
            },
            MyPapers = new UserMyPapersKpis
            {
                Total = labData.TotalPapers,
                BySubmissionStatus = paperBySubmission
            }
        };
    }

    private static string MapTaskStatus(int status) => status switch
    {
        1 => "todo",
        2 => "inProgress",
        3 => "inReview",
        4 => "completed",
        5 => "closed",
        _ => status.ToString()
    };

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

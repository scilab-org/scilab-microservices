using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Dashboard;

public sealed class SubmissionStatusCountItem
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public sealed class RecentPaperSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? Status { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class AdminDashboardKpisResult
{
    public long PaperBankTotal { get; set; }
    public long JournalTotal { get; set; }
    public long ConferenceTotal { get; set; }
    public long TemplateTotal { get; set; }
    public List<SubmissionStatusCountItem> SubmissionStatusCounts { get; set; } = [];
    public List<RecentPaperSummary> RecentPapers { get; set; } = [];
}

public record GetAdminDashboardKpisQuery : IQuery<AdminDashboardKpisResult>;

public sealed class GetAdminDashboardKpisQueryHandler(IDocumentSession session)
    : IQueryHandler<GetAdminDashboardKpisQuery, AdminDashboardKpisResult>
{
    public async Task<AdminDashboardKpisResult> Handle(
        GetAdminDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        var paperBankTotal = await session.Query<PaperBankEntity>()
            .CountAsync(cancellationToken);

        var journalTotal = await session.Query<ConferenceJournalEntity>()
            .Where(x => x.Type == ConferenceJournalType.Journal)
            .CountAsync(cancellationToken);

        var conferenceTotal = await session.Query<ConferenceJournalEntity>()
            .Where(x => x.Type == ConferenceJournalType.Conference)
            .CountAsync(cancellationToken);

        var templateTotal = await session.Query<TemplateEntity>()
            .CountAsync(cancellationToken);

        var totalPaperCount = await session.Query<PaperEntity>()
            .CountAsync(cancellationToken);

        var recentPapers = await session.Query<PaperEntity>()
            .OrderByDescending(x => x.CreatedOnUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var allHistories = await session.Query<PaperStatusHistoryEntity>()
            .OrderByDescending(h => h.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var latestPerPaper = allHistories
            .GroupBy(h => h.PaperId)
            .Select(g => g.First())
            .ToList();

        var papersWithHistory = latestPerPaper.Select(h => h.PaperId).ToHashSet();
        var draftCount = (int)totalPaperCount - papersWithHistory.Count;

        var submissionStatusCounts = latestPerPaper
            .GroupBy(h => h.Status)
            .Select(g => new SubmissionStatusCountItem
            {
                Status = (int)g.Key,
                Count = g.Count()
            })
            .ToList();

        if (draftCount > 0)
        {
            var existing = submissionStatusCounts.FirstOrDefault(x => x.Status == (int)SubmissionStatus.Draft);
            if (existing != null)
                existing.Count += draftCount;
            else
                submissionStatusCounts.Add(new SubmissionStatusCountItem
                {
                    Status = (int)SubmissionStatus.Draft,
                    Count = draftCount
                });
        }

        submissionStatusCounts.Sort((a, b) => a.Status.CompareTo(b.Status));

        return new AdminDashboardKpisResult
        {
            PaperBankTotal = paperBankTotal,
            JournalTotal = journalTotal,
            ConferenceTotal = conferenceTotal,
            TemplateTotal = templateTotal,
            SubmissionStatusCounts = submissionStatusCounts,
            RecentPapers = recentPapers.Select(p => new RecentPaperSummary
            {
                Id = p.Id,
                Title = p.Title,
                Status = p.Status.HasValue ? (int)p.Status.Value : null,
                ConferenceJournalName = p.ConferenceJournalName,
                CreatedAt = p.CreatedOnUtc
            }).ToList()
        };
    }
}

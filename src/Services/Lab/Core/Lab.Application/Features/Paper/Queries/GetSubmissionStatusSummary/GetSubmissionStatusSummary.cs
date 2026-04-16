using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Paper.Queries.GetSubmissionStatusSummary;

public sealed class SubmissionStatusSummaryItem
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public sealed class GetSubmissionStatusSummaryResult
{
    public List<SubmissionStatusSummaryItem> Items { get; set; } = [];
}

public sealed class GetSubmissionStatusSummaryRequest
{
    public List<Guid> PaperIds { get; set; } = [];
}

public record GetSubmissionStatusSummaryQuery(List<Guid> PaperIds)
    : ICommand<GetSubmissionStatusSummaryResult>;

public class GetSubmissionStatusSummaryQueryHandler(IDocumentSession session)
    : ICommandHandler<GetSubmissionStatusSummaryQuery, GetSubmissionStatusSummaryResult>
{
    public async Task<GetSubmissionStatusSummaryResult> Handle(
        GetSubmissionStatusSummaryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PaperIds.Count == 0)
            return new GetSubmissionStatusSummaryResult();

        // Single DB query — fetch all history records for the given papers
        var histories = await session.Query<PaperStatusHistoryEntity>()
            .Where(h => request.PaperIds.Contains(h.PaperId))
            .OrderByDescending(h => h.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        // Keep only the latest record per paper (list is already ordered descending)
        var latestPerPaper = histories
            .GroupBy(h => h.PaperId)
            .Select(g => g.First())
            .ToList();

        // Papers with no history are implicitly Draft
        var papersWithHistory = latestPerPaper.Select(h => h.PaperId).ToHashSet();
        var draftCount = request.PaperIds.Count(id => !papersWithHistory.Contains(id));

        // Aggregate by status
        var items = latestPerPaper
            .GroupBy(h => h.Status)
            .Select(g => new SubmissionStatusSummaryItem
            {
                Status = (int)g.Key,
                Count = g.Count(),
            })
            .ToList();

        if (draftCount > 0)
        {
            var existing = items.FirstOrDefault(x => x.Status == (int)SubmissionStatus.Draft);
            if (existing != null)
                existing.Count += draftCount;
            else
                items.Add(new SubmissionStatusSummaryItem
                {
                    Status = (int)SubmissionStatus.Draft,
                    Count = draftCount,
                });
        }

        items.Sort((a, b) => a.Status.CompareTo(b.Status));

        return new GetSubmissionStatusSummaryResult { Items = items };
    }
}

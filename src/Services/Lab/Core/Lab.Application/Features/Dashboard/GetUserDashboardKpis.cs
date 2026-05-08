using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Dashboard;

public sealed class TaskStatusCountItem
{
    public int Status { get; set; }
    public int Count { get; set; }
}

public sealed class RecentTaskSummary
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TaskType { get; set; }
    public int Status { get; set; }
    public Guid? PaperId { get; set; }
    public string? PaperTitle { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
}

public sealed class UserRecentPaperSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? PaperStatus { get; set; }
    public int SubmissionStatus { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
}

public sealed class UserDashboardKpisResult
{
    public long TotalTasks { get; set; }
    public List<TaskStatusCountItem> TaskStatusCounts { get; set; } = [];
    public long TotalPapers { get; set; }
    public List<SubmissionStatusCountItem> PaperSubmissionStatusCounts { get; set; } = [];
    public List<RecentTaskSummary> RecentTasks { get; set; } = [];
    public List<UserRecentPaperSummary> RecentPapers { get; set; } = [];
}

public record GetUserDashboardKpisQuery(string Username, Guid[] MemberIds)
    : IQuery<UserDashboardKpisResult>;

public sealed class GetUserDashboardKpisQueryHandler(IDocumentSession session)
    : IQueryHandler<GetUserDashboardKpisQuery, UserDashboardKpisResult>
{
    public async Task<UserDashboardKpisResult> Handle(
        GetUserDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Load all tasks assigned to this user
        var tasks = await session.Query<TaskEntity>()
            .Where(x => x.AssignedToUserName == request.Username)
            .ToListAsync(cancellationToken);

        var taskStatusCounts = tasks
            .GroupBy(t => t.Status)
            .Select(g => new TaskStatusCountItem { Status = (int)g.Key, Count = g.Count() })
            .OrderBy(x => x.Status)
            .ToList();

        var top5Tasks = tasks
            .OrderByDescending(t => t.LastModifiedOnUtc)
            .Take(5)
            .ToList();

        // 2. Load contributor and author records for the user's members
        var memberIds = request.MemberIds;
        var paperIds = new HashSet<Guid>();
        var taskIdToPaperId = new Dictionary<Guid, Guid>();

        if (memberIds.Length > 0)
        {
            var contributors = await session.Query<PaperContributorEntity>()
                .Where(x => memberIds.Contains(x.MemberId))
                .ToListAsync(cancellationToken);

            foreach (var c in contributors)
            {
                paperIds.Add(c.PaperId);
                foreach (var tid in c.TaskIds)
                    taskIdToPaperId.TryAdd(tid, c.PaperId);
            }

            var authorships = await session.Query<PaperAuthorEntity>()
                .Where(x => memberIds.Contains(x.MemberId))
                .ToListAsync(cancellationToken);

            foreach (var a in authorships)
                paperIds.Add(a.PaperId);
        }

        // 3. Resolve paper titles for the top-5 recent tasks
        var taskPaperIds = top5Tasks
            .Select(t => taskIdToPaperId.TryGetValue(t.Id, out var pid) ? pid : (Guid?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .Distinct()
            .ToList();

        Dictionary<Guid, string?> taskPaperTitleMap = [];
        if (taskPaperIds.Count > 0)
        {
            var taskPapers = await session.Query<PaperEntity>()
                .Where(x => taskPaperIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Title })
                .ToListAsync(cancellationToken);
            taskPaperTitleMap = taskPapers.ToDictionary(p => p.Id, p => (string?)p.Title);
        }

        var recentTasks = top5Tasks.Select(t =>
        {
            var paperId = taskIdToPaperId.TryGetValue(t.Id, out var pid) ? pid : (Guid?)null;
            return new RecentTaskSummary
            {
                Id = t.Id,
                Name = t.Name,
                TaskType = (int)t.TaskType,
                Status = (int)t.Status,
                PaperId = paperId,
                PaperTitle = paperId.HasValue && taskPaperTitleMap.TryGetValue(paperId.Value, out var title) ? title : null,
                NextReviewDate = t.NextReviewDate,
                LastModifiedAt = t.LastModifiedOnUtc
            };
        }).ToList();

        if (paperIds.Count == 0)
        {
            return new UserDashboardKpisResult
            {
                TotalTasks = tasks.Count,
                TaskStatusCounts = taskStatusCounts,
                RecentTasks = recentTasks
            };
        }

        var paperIdArray = paperIds.ToArray();

        // 4. Compute submission status counts for user's papers
        var allHistories = await session.Query<PaperStatusHistoryEntity>()
            .Where(x => paperIdArray.Contains(x.PaperId))
            .OrderByDescending(h => h.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var latestPerPaper = allHistories
            .GroupBy(h => h.PaperId)
            .Select(g => g.First())
            .ToList();

        var papersWithHistory = latestPerPaper.Select(h => h.PaperId).ToHashSet();
        var draftCount = paperIds.Count - papersWithHistory.Count;

        var submissionStatusCounts = latestPerPaper
            .GroupBy(h => h.Status)
            .Select(g => new SubmissionStatusCountItem { Status = (int)g.Key, Count = g.Count() })
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

        var latestStatusByPaper = latestPerPaper.ToDictionary(h => h.PaperId, h => (int)h.Status);

        // 5. Load top-5 most recently modified papers
        var recentPaperEntities = await session.Query<PaperEntity>()
            .Where(x => paperIdArray.Contains(x.Id))
            .OrderByDescending(x => x.LastModifiedOnUtc)
            .Take(5)
            .ToListAsync(cancellationToken);

        var recentPapers = recentPaperEntities.Select(p => new UserRecentPaperSummary
        {
            Id = p.Id,
            Title = p.Title,
            PaperStatus = p.Status.HasValue ? (int)p.Status.Value : null,
            SubmissionStatus = latestStatusByPaper.TryGetValue(p.Id, out var ss) ? ss : (int)SubmissionStatus.Draft,
            ConferenceJournalName = p.ConferenceJournalName,
            ConferenceJournalEndAt = p.ConferenceJournalEndAt,
            LastModifiedAt = p.LastModifiedOnUtc
        }).ToList();

        return new UserDashboardKpisResult
        {
            TotalTasks = tasks.Count,
            TaskStatusCounts = taskStatusCounts,
            TotalPapers = paperIds.Count,
            PaperSubmissionStatusCounts = submissionStatusCounts,
            RecentTasks = recentTasks,
            RecentPapers = recentPapers
        };
    }
}

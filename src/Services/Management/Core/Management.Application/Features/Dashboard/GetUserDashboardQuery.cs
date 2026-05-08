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
    public Dictionary<string, long> ByStatus { get; set; } = [];
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
    ILabApiService labApiService)
    : IQueryHandler<GetUserDashboardQuery, UserDashboardResult>
{

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

        // 2. Load top-level projects the user belongs to (with status breakdown)
        IReadOnlyList<ProjectEntity> userProjects = [];
        long totalProjects = 0;
        long activeProjects = 0;
        if (projectIds.Count > 0)
        {
            userProjects = await session.Query<ProjectEntity>()
                .Where(x => projectIds.Contains(x.Id) && x.ParentProjectId == null)
                .ToListAsync(cancellationToken);

            totalProjects = userProjects.Count;
            activeProjects = userProjects.Count(x => x.Status == ProjectStatus.Active);
        }

        // 3. Fetch Lab KPIs (tasks + papers); cache the KPI block per user
        var labData = await labApiService.GetUserDashboardKpisAsync(request.Username, memberIds, cancellationToken);

        // Resolve ProjectId for each recent paper using the member-to-project mapping
        var memberToProjectMap = members.ToDictionary(m => m.Id, m => m.ProjectId);
        foreach (var paper in labData.RecentPapers)
        {
            if (paper.MemberId.HasValue && memberToProjectMap.TryGetValue(paper.MemberId.Value, out var projectId))
                paper.ProjectId = projectId;
        }

        var kpis = BuildKpis(totalProjects, activeProjects, userProjects, labData);

        return new UserDashboardResult
        {
            Kpis = kpis,
            MyRecentTasks = labData.RecentTasks,
            MyRecentPapers = labData.RecentPapers
        };
    }

    private static UserDashboardKpis BuildKpis(long totalProjects, long activeProjects, IReadOnlyList<ProjectEntity> userProjects, LabUserDashboardKpisDto labData)
    {
        var taskByStatus = labData.TaskStatusCounts
            .ToDictionary(x => MapTaskStatus(x.Status), x => x.Count);

        var paperBySubmission = labData.PaperSubmissionStatusCounts
            .ToDictionary(x => MapSubmissionStatus(x.Status), x => x.Count);

        var projectByStatus = userProjects
            .GroupBy(x => x.Status)
            .ToDictionary(g => g.Key.ToString().ToLowerInvariant(), g => (long)g.Count());

        return new UserDashboardKpis
        {
            MyProjects = new UserMyProjectsKpis
            {
                Total = totalProjects,
                Active = activeProjects,
                ByStatus = projectByStatus
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

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Dashboard;

[ExcludeFromCodeCoverage]
public sealed class LabUserDashboardKpisDto
{
    public long TotalTasks { get; set; }
    public List<LabTaskStatusCountDto> TaskStatusCounts { get; set; } = [];
    public long TotalPapers { get; set; }
    public List<LabSubmissionStatusCountDto> PaperSubmissionStatusCounts { get; set; } = [];
    public List<LabRecentTaskDto> RecentTasks { get; set; } = [];
    public List<LabUserRecentPaperDto> RecentPapers { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class LabTaskStatusCountDto
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class LabRecentTaskDto
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

[ExcludeFromCodeCoverage]
public sealed class LabUserRecentPaperDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? PaperStatus { get; set; }
    public int SubmissionStatus { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
}

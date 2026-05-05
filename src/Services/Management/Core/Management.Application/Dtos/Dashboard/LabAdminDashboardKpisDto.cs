using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Dashboard;

[ExcludeFromCodeCoverage]
public sealed class LabAdminDashboardKpisDto
{
    public long PaperBankTotal { get; set; }
    public long JournalTotal { get; set; }
    public long ConferenceTotal { get; set; }
    public long TemplateTotal { get; set; }
    public List<LabSubmissionStatusCountDto> SubmissionStatusCounts { get; set; } = [];
    public List<LabRecentPaperDto> RecentPapers { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public sealed class LabSubmissionStatusCountDto
{
    public int Status { get; set; }
    public int Count { get; set; }
}

[ExcludeFromCodeCoverage]
public sealed class LabRecentPaperDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public int? Status { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

/// <summary>
/// Full paper DTO — mirrors Lab.Application.Dtos.Papers.PaperDto (GET /papers/{id}).
/// Paper entity fields only; PaperBank fields (Authors, Publisher, Doi, etc.) are not present.
/// </summary>
[ExcludeFromCodeCoverage]
public class PaperInfoDto : PaperBankInfoDto
{
    #region Fields, Properties and Indexers

    public string? Template { get; set; }
    public string? Context { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? ResearchAim { get; set; }
    public string? Rule { get; set; }
    public int? Status { get; set; }
    public int? SubmissionStatus { get; set; }
    public DateTimeOffset? ConferenceJournalStartAt { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }

    #endregion
}

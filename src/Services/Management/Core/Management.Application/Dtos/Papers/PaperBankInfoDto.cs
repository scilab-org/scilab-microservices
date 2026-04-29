using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Papers;

[ExcludeFromCodeCoverage]
public class PaperBankInfoDto
{
    #region Fields, Properties and Indexers

    public Guid Id { get; set; }
    public Guid? SubProjectId { get; set; }
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? Url { get; set; }
    public string? FilePath { get; set; }
    public string? BibFilePath { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public string? ConferenceJournalName { get; set; }
    public int? ConferenceJournalType { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> Keywords { get; set; } = new();
    public int? IngestStatus { get; set; }
    public string? CreatedBy { get; set; }

    #endregion
}

using Lab.Domain.Enums;

namespace Lab.Application.Dtos.PaperBanks;

public class UpdatePaperBankDto
{
    #region Fields, Properties and Indexers

    public string? Title { get; init; } = null!;
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; init; }
    public string? Doi { get; init; }
    public string? Url { get; init; }
    public string? Code { get; init; }
    public bool? IsIngested { get; init; }
    public bool? IsAutoTagged { get; init; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; init; }
    public string? Pages { get; init; }
    public string? Number { get; set; }
    public string? Volume { get; init; }
    public Guid ConferenceJournalId { get; init; }
    public string? ReferenceContent { get; init; }
    public List<string>? Keywords { get; init; }
    public IngestStatus? IngestStatus { get; init; }

    #endregion
}
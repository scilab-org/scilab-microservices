using Lab.Application.Dtos.Abstractions;
using Lab.Application.Dtos.GapTypes;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.PaperBanks;

public class PaperBankInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

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
    public List<GapTypeInfoDto> GapTypes { get; set; } = new();
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public string? ConferenceJournalName { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string> Keywords { get; set; } = new();
    public IngestStatus? IngestStatus { get; set; }

    #endregion
}
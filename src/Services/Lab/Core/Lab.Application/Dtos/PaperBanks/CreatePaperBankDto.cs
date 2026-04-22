namespace Lab.Application.Dtos.PaperBanks;

public class CreatePaperBankDto
{
    #region Fields, Properties and Indexers

    public string Title { get; init; } = null!;
    public string? Authors { get; init; }
    public string? Publisher { get; init; }
    public string? Ranking { get; init; }
    public string? Abstract { get; init; }
    public string? Doi { get; init; }
    public UploadFileBytes UploadPdfFile { get; set; } = null!;
    public UploadFileBytes UploadBibFile { get; set; } = null!;
    public string? Url { get; init; }
    public string? Code { get; init; }
    public string? ParsedText { get; init; }
    public bool? IsIngested { get; init; }
    public bool? IsAutoTagged { get; init; }
    public DateTimeOffset? PublicationDate { get; init; }
    public string? PaperType { get; init; }
    public string? Pages { get; init; }
    public string? Number { get; init; }
    public string? Volume { get; init; }
    public Guid ConferenceJournalId { get; init; }
    public string? ReferenceContent { get; init; }
    public List<string>? Keywords { get; init; }

    #endregion
}
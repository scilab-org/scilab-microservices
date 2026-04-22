namespace Lab.Api.Models.PaperBank;

public class UpdatePaperBankRequest
{
    #region Fields, Properties and Indexers

    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? Url { get; set; }
    public string? Code { get; set; }
    public bool? IsIngested { get; init; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public Guid ConferenceJournalId { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string>? Keywords { get; set; }

    #endregion
}
using Lab.Domain.Enums;

namespace Lab.Api.Models.PaperBank;

public class CreatePaperBankRequest
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? Ranking { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public IFormFile? File { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? Pages { get; set; }
    public string? Number { get; set; }
    public string? Volume { get; set; }
    public string? ConferenceName { get; set; }
    public string? ReferenceContent { get; set; }
    public List<string>? TagNames { get; set; }

    #endregion
}
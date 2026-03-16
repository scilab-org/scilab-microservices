namespace Management.Application.Models.Filters;

public class GetPaperBanksFilter
{
    public string? Title { get; set; } = null!;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public int? Status { get; set; }
    public DateTimeOffset? FromPublicationDate { get; set; }
    public DateTimeOffset? ToPublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
    public string[]? Tag { get; set; }
}
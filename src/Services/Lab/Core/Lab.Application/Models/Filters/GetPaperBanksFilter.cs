namespace Lab.Application.Models.Filters;

public record GetPaperBanksFilter
{
    public string? Title { get; set; } = null!;
    public string[]? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public DateTimeOffset? FromPublicationDate { get; set; }
    public DateTimeOffset? ToPublicationDate { get; set; }
    public string? PaperType { get; set; }
    public Guid? JournalId { get; set; }
    public string? Ranking { get; set; }
    public string[]? Keyword { get; set; }
    public Guid[]? ExistingPaperIds { get; set; }
    public bool? IsDeleted { get; set; } = false;
}
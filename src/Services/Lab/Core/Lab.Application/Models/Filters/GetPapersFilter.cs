using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public record class GetPapersFilter
{
    public string? Title { get; set; } = null!;
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public PaperStatus? Status { get; set; }
    public DateTimeOffset? FromPublicationDate { get; set; }
    public DateTimeOffset? ToPublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
}
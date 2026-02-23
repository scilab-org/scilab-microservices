using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class UpdatePaperDto
{
    #region Fields, Properties and Indexers

    public string? Title { get; init; } = null!;
    public string? Abstract { get; init; }
    public string? Doi { get; init; }
    public PaperStatus? Status { get; init; }
    public bool? IsAutoTagged { get; init; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; init; }
    public string? JournalName { get; init; }
    public string? ConferenceName { get; init; }
    public List<string>? TagNames { get; init; }

    #endregion
}
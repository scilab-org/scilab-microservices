using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class PaperInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string? Title { get; set; }
    public string? Abstract { get; set; }
    public string? Doi { get; set; }
    public string? FilePath { get; set; }
    public PaperStatus? Status { get; set; }
    public string? ParsedText { get; set; }
    public bool? IsIngested { get; set; }
    public bool? IsAutoTagged { get; set; }
    public DateTimeOffset? PublicationDate { get; set; }
    public string? PaperType { get; set; }
    public string? JournalName { get; set; }
    public string? ConferenceName { get; set; }
    public List<string> TagNames { get; set; } = new();

    #endregion
}
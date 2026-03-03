using Lab.Application.Dtos.Sections;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class InitPaperDto
{
    #region Fields, Properties and Indexers

    public Guid ProjectId { get; init; }
    public string Title { get; init; } = null!;
    public string? Abstract { get; init; }
    public string? Doi { get; init; }
    public PaperStatus? Status { get; init; } = PaperStatus.Draft;
    public string? PaperType { get; init; }
    public List<CreateSectionDto>? Sections { get; init; }

    #endregion
}
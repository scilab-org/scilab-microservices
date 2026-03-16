using Lab.Application.Dtos.Sections;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class CreatePaperDto
{
    #region Fields, Properties and Indexers

    public Guid ProjectId { get; init; }
    public string Title { get; init; } = null!;
    public string? Template { get; init; }
    public string Context { get; init; } = null!;
    public PaperStatus? Status { get; init; } = PaperStatus.Draft;
    public string? PaperType { get; init; }
    public List<CreateSectionDto>? Sections { get; init; }

    #endregion
}
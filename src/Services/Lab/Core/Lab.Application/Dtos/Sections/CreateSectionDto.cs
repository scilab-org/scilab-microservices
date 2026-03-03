namespace Lab.Application.Dtos.Sections;

public class CreateSectionDto
{
    #region Fields, Properties and Indexers

    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; } = null!;
    public bool? Numbered { get; init; } = true;
    public float DisplayOrder { get; init; }
    public string? SectionSumary { get; init; }
    public Guid? ParentSectionId { get; init; }

    #endregion
}
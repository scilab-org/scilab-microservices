namespace Lab.Application.Dtos.Sections;

public class CreateSectionDto
{
    #region Fields, Properties and Indexers

    public string? Title { get; init; }
    public float DisplayOrder { get; init; }
    public string? Description { get; init; }
    public string? MainIdea { get; init; }
    public string? SectionRule { get; init; }

    #endregion
}
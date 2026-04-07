namespace Lab.Application.Dtos.Sections;

public class UpsertSectionDto
{
    public Guid MemberId { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; } = null!;
    public bool? Numbered { get; init; } = true;
    public string? SectionSumary { get; init; }
    public Guid? ParentSectionId { get; init; }
    public List<string> Packages { get; init; } = new();
}
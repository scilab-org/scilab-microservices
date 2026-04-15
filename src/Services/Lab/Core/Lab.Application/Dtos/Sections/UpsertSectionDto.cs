namespace Lab.Application.Dtos.Sections;

public class UpsertSectionDto
{
    public Guid MemberId { get; init; }
    public string? Title { get; init; }
    public string? Content { get; init; } = null!;
    public string? SectionSumary { get; init; }
    public string? MainIdea { get; init; }
    public List<string> CurrentSectionPackages { get; init; } = new();
    public List<string> ReferencesPackages { get; init; } = new();
}
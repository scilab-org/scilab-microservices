namespace Management.Application.Dtos.Papers;

/// <summary>
/// Minimal representation of a Lab section returned from the Lab service.
/// </summary>
public sealed class LabSectionDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public float DisplayOrder { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid PaperId { get; set; }
}


using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Sections;

public class SectionDto : DtoId<Guid>
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? SectionSumary { get; set; }
    public string? Description { get; set; }
    public string? MainIdea { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public float DisplayOrder { get; set; }
    public string? FilePath { get; set; }
    public Guid PaperId { get; set; }
    public string SectionRole { get; set; } = "project:manager";
    public bool? IsOldMainSection { get; set; } = false;
    public bool? IsMainSection { get; set; } = false;
    public string? Rule { get; set; }
    public string? Version { get; set; }
    public List<string>? Packages { get; set; }
    public SectionStatus? Status { get; set; }
}

public class AssignedSectionDto : SectionDto
{
    public Guid PaperContributorId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
}
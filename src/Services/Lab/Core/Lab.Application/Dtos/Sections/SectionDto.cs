using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Sections;

public class SectionDto : DtoId<Guid>
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? SectionSumary { get; set; }
    public float DisplayOrder { get; set; }
    public bool? Numbered { get; set; }
    public string? FilePath { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid PaperId { get; set; }
}

public class AssignedSectionDto : SectionDto
{
    public Guid PaperContributorId { get; set; }
    public string SectionRole { get; set; } = null!;
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
}


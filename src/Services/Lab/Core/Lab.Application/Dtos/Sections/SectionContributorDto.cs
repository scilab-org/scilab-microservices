namespace Lab.Application.Dtos.Sections;

public class SectionContributorDto
{
    //Contributor info
    public Guid MemberId { get; set; }
    public string SectionRole { get; set; } = null!;
    public Guid? SectionId { get; set; }
    public Guid MarkSectionId { get; set; }
    
    //Section info
    public string? Title { get; set; }
    public bool IsMainSection { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid? PreviousVersionSectionId { get; set; }
    public Guid? NextVersionSectionId { get; set; }
    
    //User info
    public string? Name { get; set; }
    public string? Email { get; set; }
    
    public string? Content { get; set; } = null!;
}
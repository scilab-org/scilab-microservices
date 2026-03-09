namespace Lab.Application.Dtos.PaperContributors;

public class CreatePaperContributorDto
{
    public string SectionRole { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
}
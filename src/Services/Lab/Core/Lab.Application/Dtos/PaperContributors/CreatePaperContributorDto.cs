namespace Lab.Application.Dtos.PaperContributors;

public class CreatePaperContributorDto
{
    public string SectionRole { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public List<Guid> MemberIds { get; set; } = [];
    public Guid MarkSectionId { get; set; }
}
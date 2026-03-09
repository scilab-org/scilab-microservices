namespace Lab.Application.Dtos.PaperContributors;

public class UpdatePaperContributorDto
{
    public string? SectionRole { get; set; } = null!;
    public Guid? SectionId { get; set; }
    public Guid? MemberId { get; set; }
    public Guid? MarkSectionId { get; set; }
}
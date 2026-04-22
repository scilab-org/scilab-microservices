namespace Lab.Application.Dtos.PaperAuthors;

public class CreatePaperAuthorDto
{
    public string Name { get; set; } = null!;
    public string? OcrId { get; set; }
    public string Email { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid AuthorRoleId { get; set; }
    public Guid MemberId { get; set; }
    public Guid ProjectId { get; set; }
}

namespace Lab.Application.Dtos.PaperAuthors;

public class UpdatePaperAuthorDto
{
    public string? Name { get; set; }
    public string? OcrId { get; set; }
    public string? Email { get; set; }
    public Guid? PaperId { get; set; }
    public Guid? AuthorRoleId { get; set; }
    public Guid? MemberId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AffiliationId { get; set; }
    public string? AffiliationName { get; set; } = null!;
}

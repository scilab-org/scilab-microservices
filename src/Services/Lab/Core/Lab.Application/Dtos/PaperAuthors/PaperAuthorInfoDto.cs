using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.PaperAuthors;

public class PaperAuthorInfoDto : DtoId<Guid>
{
    public string Name { get; set; } = null!;
    public string? OcrId { get; set; }
    public string Email { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid AuthorRoleId { get; set; }
    public string? AuthorRoleName { get; set; }
    public string? AuthorRoleDescription { get; set; }
    public Guid MemberId { get; set; }
    public Guid AffiliationId { get; set; }
    public string AffiliationName { get; set; } = null!;
}

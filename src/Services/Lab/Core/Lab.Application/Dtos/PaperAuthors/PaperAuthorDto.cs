using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.PaperAuthors;

public class PaperAuthorDto : PaperAuthorInfoDto, IAuditableDto
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}

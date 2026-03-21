using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Journals;

public class JournalDto : JournalInfoDto, IAuditableDto
{
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedOnUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
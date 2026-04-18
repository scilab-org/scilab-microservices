using Common.Models;
using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Papers;

public class PaperVersionFileDto : ICreationAuditDto
{
    public Guid Id { get; set; }
    public Guid PaperVersionId { get; set; }
    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!;
    public string? Note { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreatePaperVersionFileDto
{
    public UploadFileBytes UploadFile { get; set; } = null!;
    public string? Note { get; set; }
}

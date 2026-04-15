namespace Lab.Application.Dtos.Journals;
using Lab.Domain.Models;

/// <summary>
/// DTO for creating a new Journal Entity
/// </summary>
public class CreateJournalEntityDto
{
    public string Name { get; set; } = null!;
    public Guid TemplateId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string Style { get; set; } = null!;
    public UploadFileBytes TexUploadFile { get; set; } = null!;
    public UploadFileBytes PdfUploadFile { get; set; } = null!;
}
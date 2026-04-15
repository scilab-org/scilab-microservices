namespace Lab.Application.Dtos.Journals;

/// <summary>
/// DTO for updating an existing Journal Entity
/// </summary>
public class UpdateJournalEntityDto
{
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public string? Name { get; set; }
    public DateTimeOffset? StartAt { get; set; }
    public DateTimeOffset? EndAt { get; set; }
    public string? Style { get; set; }
    public UploadFileBytes? TexUploadFile { get; set; }
    public UploadFileBytes? PdfUploadFile { get; set; }
}
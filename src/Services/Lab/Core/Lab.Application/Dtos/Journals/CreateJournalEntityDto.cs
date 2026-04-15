namespace Lab.Application.Dtos.Journals;
using Lab.Domain.Models;

/// <summary>
/// DTO for creating a new Journal Entity
/// </summary>
public class CreateJournalEntityDto
{
    public Guid ProjectId { get; set; }
    public Guid? TemplateId { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset StartAt { get; set; }
    public required DateTimeOffset EndAt { get; set; }
    public string? Style { get; set; }
    public UploadFileBytes? TexUploadFile { get; set; }
    public UploadFileBytes? PdfUploadFile { get; set; }
    public string? TemplateCode { get; set; }
    public string? TemplateDescription { get; set; }
    public List<Section>? Sections { get; set; }
}
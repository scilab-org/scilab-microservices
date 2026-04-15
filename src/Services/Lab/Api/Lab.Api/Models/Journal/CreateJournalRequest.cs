using Lab.Domain.Models;

namespace Lab.Api.Models.Journal;

public class CreateJournalRequest
{
    public Guid ProjectId { get; set; }
    public Guid? TemplateId { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Style { get; set; }
    public string? TemplateCode { get; set; }
    public string? TemplateDescription { get; set; }
    public List<Section>? Sections { get; set; }
    public IFormFile? TexFile { get; set; }
    public IFormFile? PdfFile { get; set; }
}
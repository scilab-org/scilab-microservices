using Lab.Domain.Models;

namespace Lab.Api.Models.Journal;

public class CreateJournalRequest
{
    public string Name { get; set; } = null!;
    public Guid TemplateId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string Style { get; set; } = null!;
    public IFormFile TexFile { get; set; } = null!;
    public IFormFile PdfFile { get; set; } = null!;
}
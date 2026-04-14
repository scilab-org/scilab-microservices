namespace Lab.Api.Models.Journal;

public class CreateJournalRequest
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Style { get; set; }
    public IFormFile? TexFile { get; set; }
    public IFormFile? PdfFile { get; set; }
}
namespace Lab.Api.Models.Journal;

public class CreateJournalRequest
{
    public string Name { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Style { get; set; }
    public IFormFile? TexFile { get; set; }
    public IFormFile? PdfFile { get; set; }
}
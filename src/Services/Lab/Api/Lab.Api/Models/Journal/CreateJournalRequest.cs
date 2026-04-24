using Lab.Domain.Enums;

namespace Lab.Api.Models.Journal;

public class CreateJournalRequest
{
    public string Name { get; set; } = null!;
    public List<Guid> TemplateIds { get; set; }
    public string ISSN { get; set; } = null!;
    public ConferenceJournalType Type { get; set; }
    public string Ranking { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string Style { get; set; } = null!;
    public IFormFile? TexFile { get; set; } = null!;
    public IFormFile? PdfFile { get; set; } = null!;
}
namespace Lab.Api.Models.Journal;

public class UpdateJournalRequest
{
    public string Name {get; set;} = string.Empty;
    public string? Ranking  { get; set; }
    public string? Url { get; set; }
    public string? ISSN { get; set; }
    public string? Style { get; set; }
    public List<Guid> TemplateIds { get; set; }
    public IFormFile? TexFile { get; set; }
    public IFormFile? PdfFile { get; set; }
}
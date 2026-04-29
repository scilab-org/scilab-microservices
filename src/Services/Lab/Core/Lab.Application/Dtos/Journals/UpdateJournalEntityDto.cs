using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Journals;

/// <summary>
/// DTO for updating an existing Journal Entity
/// </summary>
public class UpdateJournalEntityDto
{
    public string Name { get; set; } = null!;
    public string? Ranking  { get; set; }
    public string? Url { get; set; }
    public string? Style { get; set; }
    public string? ISSN { get; set; }
    public List<Guid> TemplateIds { get; set; }
    public UploadFileBytes? TexUploadFile { get; set; }
    public UploadFileBytes? PdfUploadFile { get; set; }
}
using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Journals;

public class JournalInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Ranking { get; set; }
    public string? Url { get; set; }
    public string? TexFile { get; set; }
    public string? PdfFile { get; set; }
    public string? Style { get; set; }
    public Guid TemplateId { get; set; }
    public string? TemplateCode { get; set; }
    public List<Guid>? ProjectIds { get; set; }

    #endregion
}
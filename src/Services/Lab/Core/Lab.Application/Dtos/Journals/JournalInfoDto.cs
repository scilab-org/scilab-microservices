using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Journals;

public class JournalInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Ranking { get; set; }
    public string? Url { get; set; }
    public string? ISSN { get; set; }
    public string? TexFile { get; set; }
    public string? PdfFile { get; set; }
    public string? Style { get; set; }
    public ConferenceJournalType Type { get; set; }
    public List<JournalTemplateDto> Templates { get; set; } = [];

    #endregion
}
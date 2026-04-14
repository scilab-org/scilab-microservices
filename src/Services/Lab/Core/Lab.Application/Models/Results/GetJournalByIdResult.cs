using Lab.Application.Dtos.Journals;
using Lab.Application.Dtos.Template;

namespace Lab.Application.Models.Results;

public class GetJournalByIdResult
{
    #region Fields, Properties and Indexers

    public JournalDto Journal { get; init; }
    public List<TemplateDto> Templates { get; init; } = [];

    #endregion

    #region Ctors
    public GetJournalByIdResult(JournalDto journal, List<TemplateDto>? templates = null)
    {
        Journal = journal;
        Templates = templates ?? [];
    }

    #endregion
}
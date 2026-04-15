using Lab.Application.Dtos.Journals;

namespace Lab.Application.Models.Results;

public class GetJournalByIdResult
{
    #region Fields, Properties and Indexers

    public JournalDto Journal { get; init; }
    public List<ProjectJournalInfo> Projects { get; init; }

    #endregion

    #region Ctors

    public GetJournalByIdResult(JournalDto journal, List<ProjectJournalInfo>? projects)
    {
        Journal = journal;
        Projects = projects ?? [];
    }

    #endregion
}
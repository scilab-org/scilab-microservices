using Lab.Application.Dtos.Journals;

namespace Lab.Application.Models.Results;

public class GetJournalByIdResult
{
    #region Fields, Properties and Indexers

    public JournalDto Journal { get; init; }
    public List<ProjectJournalInfo> Projects { get; init; }
    public ProjectJournalInfo? Project => Projects.Count > 0 ? Projects[0] : null;

    #endregion

    #region Ctors

    public GetJournalByIdResult(JournalDto journal, List<ProjectJournalInfo>? projects)
    {
        Journal = journal;
        Projects = projects ?? [];
    }

    public GetJournalByIdResult(JournalDto journal, ProjectJournalInfo? project)
        : this(journal, project is null ? [] : [project])
    {
    }

    #endregion
}
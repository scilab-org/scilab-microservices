using Lab.Application.Dtos.Journals;

namespace Lab.Application.Models.Results;

public class GetJournalByIdResult
{
    #region Fields, Properties and Indexers

    public JournalDto Journal { get; init; }

    #endregion

    #region Ctors
    public GetJournalByIdResult(JournalDto journal)
    {
        Journal = journal;
    }

    #endregion
}
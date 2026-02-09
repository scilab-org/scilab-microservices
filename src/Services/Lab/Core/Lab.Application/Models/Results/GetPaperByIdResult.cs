using Lab.Application.Dtos.Papers;

namespace Lab.Application.Models.Results;

public class GetPaperByIdResult
{
    #region Fields, Properties and Indexers

    public PaperDto Paper { get; init; }

    #endregion

    #region Ctors
    public GetPaperByIdResult(PaperDto paper)
    {
        Paper = paper;
    }

    #endregion
}
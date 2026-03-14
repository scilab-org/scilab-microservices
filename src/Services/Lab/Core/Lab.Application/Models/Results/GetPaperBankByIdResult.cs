using Lab.Application.Dtos.PaperBanks;

namespace Lab.Application.Models.Results;

public class GetPaperBankByIdResult
{
    #region Fields, Properties and Indexers

    public PaperDto Paper { get; init; }

    #endregion

    #region Ctors
    public GetPaperBankByIdResult(PaperDto paper)
    {
        Paper = paper;
    }

    #endregion
}
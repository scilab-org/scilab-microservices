using Lab.Application.Dtos.PaperBanks;

namespace Lab.Application.Models.Results;

public class GetPaperBankByIdResult
{
    #region Fields, Properties and Indexers

    public PaperBankDto PaperBank { get; init; }

    #endregion

    #region Ctors
    public GetPaperBankByIdResult(PaperBankDto paperBank)
    {
        PaperBank = paperBank;
    }

    #endregion
}
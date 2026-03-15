using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetAvailablePapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperBankInfoDto> Items { get; init; }
    public int TotalCount { get; init; }

    #endregion

    #region Ctors

    public GetAvailablePapersResult(List<PaperBankInfoDto> items)
    {
        Items = items;
        TotalCount = items.Count;
    }

    #endregion
}

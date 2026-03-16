using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetAvailablePapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperBankInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetAvailablePapersResult(
        List<PaperBankInfoDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}

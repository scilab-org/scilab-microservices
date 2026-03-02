using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetSubProjectsPapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetSubProjectsPapersResult(
        List<PaperInfoDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}

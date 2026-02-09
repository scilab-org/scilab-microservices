using Lab.Application.Dtos.Papers;

namespace Lab.Application.Models.Results;

public sealed class GetPapersResult
{
    #region Fields, Properties and Indexers

    public List<PaperDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetPapersResult(
        List<PaperDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}
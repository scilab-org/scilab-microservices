using Lab.Application.Dtos.Tags;

namespace Lab.Application.Models.Results;

public sealed class GetTagsResult
{
    #region Fields, Properties and Indexers

    public List<TagDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetTagsResult(
        List<TagDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}
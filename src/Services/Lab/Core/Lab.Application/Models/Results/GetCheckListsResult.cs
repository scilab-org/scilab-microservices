using Lab.Application.Dtos.CheckLists;

namespace Lab.Application.Models.Results;

public sealed class GetCheckListsResult
{
    public List<CheckListDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetCheckListsResult(
        List<CheckListDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

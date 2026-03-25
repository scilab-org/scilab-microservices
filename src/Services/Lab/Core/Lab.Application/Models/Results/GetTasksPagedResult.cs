using Lab.Application.Dtos.Tasks;
using BuildingBlocks.Pagination;

namespace Lab.Application.Models.Results;

public sealed class GetTasksPagedResult
{
    public List<TaskDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetTasksPagedResult(List<TaskDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

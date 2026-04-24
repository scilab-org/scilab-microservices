using Lab.Application.Dtos.GapTypes;

namespace Lab.Application.Models.Results;

public sealed class GetGapTypesResult
{
    public List<GapTypeDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetGapTypesResult(List<GapTypeDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

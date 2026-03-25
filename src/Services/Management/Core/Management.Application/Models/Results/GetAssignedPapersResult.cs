using Management.Application.Dtos.Papers;

namespace Management.Application.Models.Results;

public sealed class GetAssignedPapersResult
{
    public List<PaperInfoDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetAssignedPapersResult(List<PaperInfoDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

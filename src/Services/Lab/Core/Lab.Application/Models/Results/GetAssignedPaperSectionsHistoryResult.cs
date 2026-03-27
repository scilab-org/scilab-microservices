using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetAssignedPaperSectionsHistoryResult
{
    public List<AssignedSectionHistoryItemDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetAssignedPaperSectionsHistoryResult(
        List<AssignedSectionHistoryItemDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

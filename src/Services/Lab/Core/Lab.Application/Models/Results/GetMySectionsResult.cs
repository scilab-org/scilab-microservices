using Lab.Application.Dtos.Sections;

namespace Lab.Application.Models.Results;

public class GetMySectionsResult
{
    public Guid PaperId { get; init; }
    public Guid SubProjectId { get; init; }
    public Guid MemberId { get; init; }
    public List<AssignedSectionDto> Items { get; init; }
    public PagingResult Paging { get; init; }
    
    #region Ctors

    public GetMySectionsResult(
        Guid paperId,
        Guid subProjectId,
        Guid memberId,
        List<AssignedSectionDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        PaperId = paperId;
        SubProjectId = subProjectId;
        MemberId = memberId;
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}


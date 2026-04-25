using BuildingBlocks.Pagination;
using Common.Models;
using Management.Application.Dtos.Affiliations;

namespace Management.Application.Models.Results;

public sealed class GetAffiliationsResult
{
    public List<AffiliationDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetAffiliationsResult(List<AffiliationDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

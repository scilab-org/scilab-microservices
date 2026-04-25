using BuildingBlocks.Pagination;
using Common.Models;
// cspell:disable-next-line
using Management.Application.Dtos.UserAffiliations;

namespace Management.Application.Models.Results;

public sealed class GetMemberAffiliationsResult
{
    public List<UserAffiliationDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetMemberAffiliationsResult(List<UserAffiliationDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

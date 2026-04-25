using BuildingBlocks.Pagination;
using Common.Models;
using Management.Application.Dtos.Domains;

namespace Management.Application.Models.Results;

public sealed class GetDomainsResult
{
    public List<DomainDto> Items { get; init; }
    public PagingResult Paging { get; init; }

    public GetDomainsResult(List<DomainDto> items, long totalCount, PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }
}

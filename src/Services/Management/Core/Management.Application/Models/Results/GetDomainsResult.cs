using Management.Application.Dtos.Domains;

namespace Management.Application.Models.Results;

public sealed record GetDomainsResult(List<DomainDto> Items, long TotalCount, PaginationRequest Paging);

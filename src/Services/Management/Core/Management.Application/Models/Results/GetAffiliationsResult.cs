using Common.Models;
using Management.Application.Dtos.Affiliations;

namespace Management.Application.Models.Results;

public sealed record GetAffiliationsResult(List<AffiliationDto> Items, long TotalCount, PaginationRequest Paging);

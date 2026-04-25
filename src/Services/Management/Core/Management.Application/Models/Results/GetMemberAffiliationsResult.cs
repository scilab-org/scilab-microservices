using Common.Models;
using Management.Application.Dtos.UserAffiliations;

namespace Management.Application.Models.Results;

public sealed record GetMemberAffiliationsResult(List<UserAffiliationDto> Items, long TotalCount, PaginationRequest Paging);

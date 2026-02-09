#region using

using User.Application.Models.Filters;
using User.Application.Models.Results;
using User.Application.Services;

#endregion

namespace User.Application.Features.Users.Queries;

public sealed record GetUsersQuery(
    GetUsersFilter Filter,
    PaginationRequest Paging) : IQuery<GetUsersResult>;

public sealed class GetUsersQueryHandler(
    IKeycloakService keycloakService) : IQueryHandler<GetUsersQuery, GetUsersResult>
{
    #region Implementations

    public async Task<GetUsersResult> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var filter = query.Filter;
        var paging = query.Paging;

        var (users, totalCount) = await keycloakService.GetUsersAsync(
            searchText: filter.SearchText,
            groupName: filter.GroupName,
            pageNumber: paging.PageNumber,
            pageSize: paging.PageSize,
            cancellationToken: cancellationToken);

        return new GetUsersResult(users, totalCount, paging);
    }

    #endregion
}

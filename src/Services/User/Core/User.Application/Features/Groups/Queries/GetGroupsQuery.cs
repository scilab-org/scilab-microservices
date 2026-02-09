#region using

using User.Application.Dtos.Groups;
using User.Application.Services;

#endregion

namespace User.Application.Features.Groups.Queries;

public sealed record GetGroupsQuery() : IQuery<List<GroupDto>>;

public sealed class GetGroupsQueryHandler(
    IKeycloakService keycloakService) : IQueryHandler<GetGroupsQuery, List<GroupDto>>
{
    #region Implementations

    public async Task<List<GroupDto>> Handle(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        return await keycloakService.GetGroupsAsync(cancellationToken);
    }

    #endregion
}

#region using

using User.Application.Dtos.Roles;
using User.Application.Services;

#endregion

namespace User.Application.Features.Roles.Queries;

public sealed record GetRealmRolesQuery() : IQuery<List<RoleDto>>;

public sealed class GetRealmRolesQueryHandler(
    IKeycloakService keycloakService) : IQueryHandler<GetRealmRolesQuery, List<RoleDto>>
{
    #region Implementations

    public async Task<List<RoleDto>> Handle(GetRealmRolesQuery query, CancellationToken cancellationToken)
    {
        return await keycloakService.GetRealmRolesAsync(cancellationToken);
    }

    #endregion
}

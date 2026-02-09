#region using

using User.Application.Dtos.Roles;
using User.Application.Services;

#endregion

namespace User.Application.Features.Roles.Queries;

public sealed record GetGroupRolesQuery(string GroupId) : IQuery<List<RoleDto>>;

public sealed class GetGroupRolesQueryValidator : AbstractValidator<GetGroupRolesQuery>
{
    #region Ctors

    public GetGroupRolesQueryValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty()
            .WithMessage(MessageCode.GroupIdIsRequired);
    }

    #endregion
}

public sealed class GetGroupRolesQueryHandler(
    IKeycloakService keycloakService) : IQueryHandler<GetGroupRolesQuery, List<RoleDto>>
{
    #region Implementations

    public async Task<List<RoleDto>> Handle(GetGroupRolesQuery query, CancellationToken cancellationToken)
    {
        return await keycloakService.GetGroupRolesAsync(
            groupId: query.GroupId,
            cancellationToken: cancellationToken);
    }

    #endregion
}

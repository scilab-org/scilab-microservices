#region using

using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Dtos.Roles;
using User.Application.Features.Roles.Queries;

#endregion

namespace User.Api.Endpoints;

public sealed class GetGroupRoles : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Roles.GetGroupRoles, HandleGetGroupRolesAsync)
            .WithTags(ApiRoutes.Groups.Tags)
            .WithName(nameof(GetGroupRoles))
            .Produces<ApiGetResponse<List<RoleDto>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<List<RoleDto>>> HandleGetGroupRolesAsync(
        ISender sender,
        [FromRoute] string groupId)
    {
        var query = new GetGroupRolesQuery(groupId);

        var result = await sender.Send(query);

        return new ApiGetResponse<List<RoleDto>>(result);
    }

    #endregion
}

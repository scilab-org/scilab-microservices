#region using

using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using User.Api.Constants;
using User.Application.Dtos.Roles;
using User.Application.Features.Roles.Queries;

#endregion

namespace User.Api.Endpoints;

public sealed class GetRealmRoles : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Roles.GetAll, HandleGetRealmRolesAsync)
            .WithTags(ApiRoutes.Roles.Tags)
            .WithName(nameof(GetRealmRoles))
            .Produces<ApiGetResponse<List<RoleDto>>>(StatusCodes.Status200OK)
            .RequireAuthorization("admin");
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<List<RoleDto>>> HandleGetRealmRolesAsync(
        ISender sender,
        IHttpContextAccessor httpContext)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin))
        {
            throw new UnauthorizedAccessException();
        }

        var query = new GetRealmRolesQuery();

        var result = await sender.Send(query);

        return new ApiGetResponse<List<RoleDto>>(result);
    }

    #endregion
}

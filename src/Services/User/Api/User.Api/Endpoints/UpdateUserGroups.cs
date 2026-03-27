using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Api.Models;
using User.Application.Features.Users;

namespace User.Api.Endpoints;

public sealed class UpdateUserGroups : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Users.UpdateGroups, HandleAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(UpdateUserGroups))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<bool>> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string userId,
        [FromBody] UpdateUserGroupsRequest req)
    {
        var currentUser = httpContext.GetCurrentUser();
        var command = new UpdateUserGroupsCommand(userId, req.GroupNames ?? [], Actor.User(currentUser.Email));
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);
    }
}

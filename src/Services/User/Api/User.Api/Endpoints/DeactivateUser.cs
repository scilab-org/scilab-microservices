#region using

using BuildingBlocks.Authentication.Extensions;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class DeactivateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Users.Deactivate, HandleDeactivateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(DeactivateUser))
            .Produces<ApiDeletedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<bool>> HandleDeactivateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string userId)
    {
        var currentUser = httpContext.GetCurrentUser();
        var command = new DeactivateUserCommand(userId, Actor.User(currentUser.Email));

        var result = await sender.Send(command);

        return new ApiDeletedResponse<bool>(result);
    }

    #endregion
}

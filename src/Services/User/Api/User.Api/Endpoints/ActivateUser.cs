#region using

using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class ActivateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Users.Activate, HandleActivateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(ActivateUser))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("admin");
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<bool>> HandleActivateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string userId)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin))
        {
            throw new UnauthorizedAccessException();
        }
        
        var command = new ActivateUserCommand(userId, Actor.User(currentUser.Email));

        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);
    }

    #endregion
}

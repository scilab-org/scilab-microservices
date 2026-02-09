#region using

using BuildingBlocks.Authentication.Extensions;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Dtos.Users;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class UpdateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Users.Update, HandleUpdateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(UpdateUser))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<bool>> HandleUpdateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string userId,
        [FromBody] UpdateUserDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        var command = new UpdateUserCommand(userId, dto, Actor.User(currentUser.Email));

        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);
    }

    #endregion
}

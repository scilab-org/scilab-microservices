#region using

using BuildingBlocks.Authentication.Extensions;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Dtos.Users;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class CreateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Users.Create, HandleCreateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(CreateUser))
            .Produces<ApiCreatedResponse<string>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiCreatedResponse<string>> HandleCreateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreateUserDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        var command = new CreateUserCommand(dto, Actor.User(currentUser.Email));

        var result = await sender.Send(command);

        return new ApiCreatedResponse<string>(result);
    }

    #endregion
}

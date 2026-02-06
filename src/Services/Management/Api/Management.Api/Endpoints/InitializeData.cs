#region using

using BuildingBlocks.Authentication.Extensions;
using Common.Models.Context;
using Management.Api.Constants;
using Management.Application.Features.System;

#endregion

namespace Management.Api.Endpoints;

public sealed class InitializeData : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.System.InitializeData, HandleInitializeDataAsync)
            .WithTags(ApiRoutes.System.Tags)
            .WithName(nameof(InitializeData))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<bool>> HandleInitializeDataAsync(
        ISender sender,
        IHttpContextAccessor httpContext)
    {
        var currentUser = httpContext.GetCurrentUser();
        var actor = currentUser is not null
            ? Actor.User(currentUser.Email)
            : Actor.System("public-api");
        var command = new InitialDataCommand(actor);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);

        // var currentUser = httpContext.GetCurrentUser();
        // return new ApiGetResponse<UserContext>(currentUser);
    }

    #endregion
}
#region using

using BuildingBlocks.Authentication.Extensions;
using Common.Models.Context;
using Lab.Api.Constants;
using Lab.Application.Features.System;

#endregion

namespace Lab.Api.Endpoints;

public sealed class InitializeData : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.System.InitializeData, HandleInitializeDataAsync)
            .WithTags(ApiRoutes.System.Tags)
            .WithName(nameof(InitializeData))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<UserContext>> HandleInitializeDataAsync(
        ISender sender,
        IHttpContextAccessor httpContext)
    {
        // var currentUser = httpContext.GetCurrentUser();
        // var command = new InitialDataCommand(Actor.User(currentUser.Email));
        // var result = await sender.Send(command);

        // return new ApiUpdatedResponse<bool>(result);
        
        var currentUser = httpContext.GetCurrentUser();
        return new ApiGetResponse<UserContext>(currentUser);
    }

    #endregion
}


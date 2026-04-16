using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.TransitionPaperStatus;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class TransitionPaperStatus : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.TransitionStatus, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(TransitionPaperStatus))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] TransitionPaperStatusDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var command = new TransitionPaperStatusCommand(id, dto, userId, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created(
            $"{ApiRoutes.Paper.GetStatusHistory.Replace("{id}", id.ToString())}",
            new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}

using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.UpdatePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdatePaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Paper.Update, HandleUpdatePaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(UpdatePaper))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleUpdatePaperAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] UpdatePaperDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new UpdatePaperCommand(dto, id, userId);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Paper.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
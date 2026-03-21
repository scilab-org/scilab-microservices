using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CreatePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreatePaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.Create, HandleCreatePaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(CreatePaper))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreatePaperAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreatePaperDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new CreatePaperCommand(dto, userId);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Paper.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
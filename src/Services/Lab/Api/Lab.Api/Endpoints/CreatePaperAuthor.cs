using BuildingBlocks.Authentication.Extensions;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Features.PaperAuthor.Commands.CreatePaperAuthor;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreatePaperAuthor : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.PaperAuthor.Create, HandleCreatePaperAuthorAsync)
            .WithTags(ApiRoutes.PaperAuthor.Tags)
            .WithName(nameof(CreatePaperAuthor))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<IResult> HandleCreatePaperAuthorAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreatePaperAuthorDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new CreatePaperAuthorCommand(dto);
        var result = await sender.Send(command);
        return TypedResults.Created($"{ApiRoutes.PaperAuthor.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

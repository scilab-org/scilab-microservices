using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.GapTypes;
using Lab.Application.Features.GapType.Commands.CreateGapType;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateGapType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.GapType.Create, HandleCreateGapTypeAsync)
            .WithTags(ApiRoutes.GapType.Tags)
            .WithName(nameof(CreateGapType))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<IResult> HandleCreateGapTypeAsync(
        ISender sender,
        [FromBody] CreateGapTypeDto dto)
    {
        var result = await sender.Send(new CreateGapTypeCommand(dto));
        return TypedResults.Created($"{ApiRoutes.GapType.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

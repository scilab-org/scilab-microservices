using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Tag.Commands.CreateTag;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateTag : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Tag.Create, HandleCreateTagAsync)
            .WithTags(ApiRoutes.Tag.Tags)
            .WithName(nameof(CreateTag))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateTagAsync(
        ISender sender,
        IMapper mapper,
        [FromBody] CreateTagCommand command)
    {
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Tag.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
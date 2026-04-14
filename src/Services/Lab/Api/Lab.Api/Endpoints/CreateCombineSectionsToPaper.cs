using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CombineSectionsToPaper;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreateCombineSectionsToPaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.Versions, HandleCreateCombineSectionsToPaper)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(CreateCombineSectionsToPaper))
            .Produces<ApiCreatedResponse<CombineSectionsToPaperResult>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateCombineSectionsToPaper(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] CreatePaperCombineDto request)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new CombineSectionsToPaperCommand(id, request, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created($"/papers/{id}/versions/{result.Version?.Id}/combine",
            new ApiCreatedResponse<CombineSectionsToPaperResult>(result));
    }

    #endregion
}
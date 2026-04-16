using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.MarkSectionToCompleted;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class MarkSectionToCompleted : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.MarkSectionToCompleted, HandleMarkSectionToCompletedAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(MarkSectionToCompleted))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleMarkSectionToCompletedAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] MarkSectionToCompletedDto request)
    {

        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new MarkSectionToCompletedCommand(id, request, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }

    #endregion
}
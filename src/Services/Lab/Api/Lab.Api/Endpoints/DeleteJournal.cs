using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Features.Journal.Commands.DeleteJournal;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class DeleteJournal : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Journal.Delete, HandleDeleteJournalAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(DeleteJournal))
            .Produces<ApiDeletedResponse<Unit>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteJournalAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromRoute] Guid projectId)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new DeleteJournalCommand(id, projectId, currentUser.UserName);
        await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<Unit>());
    }

    #endregion
}
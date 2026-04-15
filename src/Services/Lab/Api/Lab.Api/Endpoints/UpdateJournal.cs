using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.Journal;
using Lab.Application.Dtos.Journals;
using Lab.Application.Features.Journal.Commands.UpdateJournal;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdateJournal : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Journal.Update, HandleUpdateJournalAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(UpdateJournal))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleUpdateJournalAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] UpdateJournalEntityDto request)
    {
        if (request == null) throw new ClientValidationException(MessageCode.BadRequest);

        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new UpdateJournalCommand(request, id, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }

    #endregion
}
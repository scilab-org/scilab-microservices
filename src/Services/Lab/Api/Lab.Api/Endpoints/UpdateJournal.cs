using Lab.Api.Constants;
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
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleUpdateJournalAsync(
        ISender sender,
        Guid id,
        [FromBody] UpdateJournalEntityDto dto)
    {
        dto.Id = id;
        var command = new UpdateJournalCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }

    #endregion
}
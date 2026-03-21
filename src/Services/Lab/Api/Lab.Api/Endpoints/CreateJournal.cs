using Common.Models.Reponses;
using Lab.Api.Constants;
using Lab.Application.Features.Journal.Commands.CreateJournal;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateJournal : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Journal.Create, HandleCreateJournalAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(CreateJournal))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateJournalAsync(
        ISender sender,
        [FromBody] CreateJournalCommand command)
    {
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Journal.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
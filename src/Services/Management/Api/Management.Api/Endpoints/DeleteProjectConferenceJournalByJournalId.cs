using Management.Api.Constants;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteProjectConferenceJournalByJournalId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectConferenceJournal.DeleteProjectConferenceJournalByJournalId, HandleDeleteProjectConferenceJournalByJournalIdAsync)
            .WithTags(ApiRoutes.ProjectConferenceJournal.Tags)
            .WithName(nameof(DeleteProjectConferenceJournalByJournalId))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> HandleDeleteProjectConferenceJournalByJournalIdAsync(
        ISender sender,
        [FromRoute] Guid journalId)
    {
        var command = new DeleteProjectConferenceJournalByJournalIdCommand(journalId);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }
}
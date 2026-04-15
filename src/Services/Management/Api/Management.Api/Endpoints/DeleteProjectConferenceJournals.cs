using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteProjectConferenceJournals : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.ProjectConferenceJournal.ProjectConferenceJournals, HandleDeleteProjectConferenceJournalsAsync)
            .WithTags(ApiRoutes.ProjectConferenceJournal.Tags)
            .WithName(nameof(DeleteProjectConferenceJournals))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private async Task<IResult> HandleDeleteProjectConferenceJournalsAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromRoute] Guid journalId)
    {
        var command = new DeleteProjectConferenceJournalsCommand(projectId, journalId);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<Guid>(result));
    }
}
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class CreateProjectConferenceJournals : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectConferenceJournal.ProjectConferenceJournals,
                HandleCreateProjectConferenceJournalsAsync)
            .WithTags(ApiRoutes.ProjectConferenceJournal.Tags)
            .WithName(nameof(CreateProjectConferenceJournals))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private async Task<ApiCreatedResponse<Guid>> HandleCreateProjectConferenceJournalsAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromRoute] Guid journalId)
    {
        var command = new CreateProjectConferenceJournalCommand(projectId, journalId);
        var result = await sender.Send(command);
        return new ApiCreatedResponse<Guid>(result);
    }
}
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class CreateProjectConferenceJournals : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectConferenceJournal.CreateProjectConferenceJournals, HandleCreateProjectConferenceJournalsAsync)
            .WithTags(ApiRoutes.ProjectConferenceJournal.Tags)
            .WithName(nameof(CreateProjectConferenceJournals))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private async Task<ApiCreatedResponse<List<Guid>>> HandleCreateProjectConferenceJournalsAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromBody] CreateProjectConferenceJournalDto req)
    {
        var command = new CreateProjectConferenceJournalCommand(projectId, req);
        var result = await sender.Send(command);
        return new ApiCreatedResponse<List<Guid>>(result);
    }
}
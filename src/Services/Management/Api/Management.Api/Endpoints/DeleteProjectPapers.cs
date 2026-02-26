using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Member.Commands;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteProjectPapers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectPaper.DeleteProjectPapers, HandleDeleteProjectPapersAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(DeleteProjectPapers))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteProjectPapersAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromBody] DeleteProjectPaperDto req)
    {
        var command = new DeleteProjectPapersCommand(projectId, req);

        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }

    #endregion
}
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class CreateSubProject: ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectPaper.CreateSubProject, HandleCreateSubProjectAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(CreateSubProject))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }
    
    #endregion
    
    #region Methods
    
    private async Task<ApiCreatedResponse<Guid>> HandleCreateSubProjectAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromBody] CreateSubProjectDto req)
    {
        var command = new CreateSubProjectCommand(projectId, req);

        var result = await sender.Send(command);

        return new ApiCreatedResponse<Guid>(result);
    }
    
    #endregion
}
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class CreateProjectPaper : ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectPaper.CreateProjectPaper, HandleCreateProjectPaperAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(CreateProjectPaper))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }
    
    #endregion
    
    #region Methods
    
    private async Task<ApiCreatedResponse<List<Guid>>> HandleCreateProjectPaperAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromBody] CreateProjectPaperDto req)
    {
        var command = new CreateProjectPaperCommand(projectId, req);

        var result = await sender.Send(command);

        return new ApiCreatedResponse<List<Guid>>(result);
    }
    
    #endregion
}
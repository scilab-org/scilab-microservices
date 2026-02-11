using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class CreatePaperProject : ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Project.AddPaperProject, HandleAddPaperProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(CreatePaperProject))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }
    
    #endregion
    
    #region Methods
    
    private async Task<ApiCreatedResponse<List<Guid>>> HandleAddPaperProjectAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromBody] AddPaperProjectDto req)
    {
        var command = new AddPaperProjectCommand(projectId, req);

        var result = await sender.Send(command);

        return new ApiCreatedResponse<List<Guid>>(result);
    }
    
    #endregion
}
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class UpdateProject : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Project.Update, HandleUpdateProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(UpdateProject))
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion
    #region Methods
    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateProjectAsync(
        ISender sender,
        Guid projectId,
        [FromBody] UpdateProjectDto req)
    {
        var command = new UpdateProjectCommand(projectId, req);

        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
    #endregion
}
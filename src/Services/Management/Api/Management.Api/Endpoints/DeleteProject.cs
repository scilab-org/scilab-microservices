using Management.Api.Constants;
using Management.Application.Features.Project.Commands;

namespace Management.Api.Endpoints;

public class DeleteProject : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Project.Delete, HandleDeleteProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(DeleteProject))
            .Produces<ApiDeletedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteProjectAsync(
        ISender sender,
        Guid projectId)
    {
        var command = new DeleteProjectCommand(projectId);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(projectId);
    }

    #endregion
}
using Management.Api.Constants;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteSubProjectPaper: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.SubProject.DeleteSubProjectPaper, HandleDeleteSubProjectAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(DeleteSubProjectPaper))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteSubProjectAsync(
        ISender sender,
        [FromRoute] Guid subProjectId)
    {
        var command = new DeleteSubProjectCommand(subProjectId);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(subProjectId);
    }

    #endregion
}
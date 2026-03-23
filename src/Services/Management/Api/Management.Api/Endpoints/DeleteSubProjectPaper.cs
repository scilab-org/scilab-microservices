using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
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
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteSubProjectAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid subProjectId)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new DeleteSubProjectCommand(subProjectId, userId, currentUser.UserName);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(subProjectId);
    }

    #endregion
}
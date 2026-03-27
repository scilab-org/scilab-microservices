using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Features.TaskDefinition.Commands.DeleteTask;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeleteTask: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Task.Delete, HandleDeleteTask)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(DeleteTask))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteTask(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new DeleteTaskCommand(id, currentUser.Id, currentUser.UserName);
        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
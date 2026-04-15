using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Features.TaskDefinition.Commands.UpdateTask;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateTask: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Task.Update, HandleUpdateTaskAsync)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(UpdateTask))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateTaskAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] UpdateTaskDto request)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            throw new NoPermissionException(MessageCode.AccessDenied);

        var command = new UpdateTaskCommand(id, request, currentUser.UserName, currentUser.Id);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
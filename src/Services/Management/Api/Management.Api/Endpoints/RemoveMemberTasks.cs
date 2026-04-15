using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands.RemoveMemberTasks;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class RemoveMemberTasks : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Member.RemoveMemberTasks, HandleRemoveMemberTasksAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(RemoveMemberTasks))
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleRemoveMemberTasksAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromRoute] Guid memberId,
        [FromBody] MemberTaskRequestDto request)
    {
        var command = new RemoveMemberTasksCommand(memberId, request.TaskIds);
        await sender.Send(command);
        return Results.Ok();
    }
}

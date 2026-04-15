using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands.AddMemberTasks;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class AddMemberTasks : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Member.AddMemberTasks, HandleAddMemberTasksAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(AddMemberTasks))
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAddMemberTasksAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromRoute] Guid memberId,
        [FromBody] MemberTaskRequestDto request)
    {
        var command = new AddMemberTasksCommand(memberId, request.TaskIds);
        await sender.Send(command);
        return Results.Ok();
    }
}

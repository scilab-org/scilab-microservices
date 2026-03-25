using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Features.TaskDefinition.Commands.CreateTask;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreateTask: ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Task.Create, HandleAsync)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(CreateTask))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }
    #endregion

    #region Methods

    private static async Task<IResult> HandleAsync(
        IHttpContextAccessor httpContext,
        [FromBody] CreateTaskDto request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            return Results.Unauthorized();
        var command = new CreateTaskCommand(request, currentUser.Id, currentUser.UserName);
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }
    #endregion
}
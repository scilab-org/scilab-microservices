using Lab.Api.Constants;
using Lab.Application.Features.TaskDefinition.Queries.GetTasksByPaperId;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Common.Models;
using BuildingBlocks.Authentication.Extensions;

namespace Lab.Api.Endpoints;

public class GetTasksByPaperId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Task.GetTasksByPaperId, HandleAsync)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(GetTasksByPaperId))
            .Produces<ApiGetResponse<GetTasksPagedResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid paperId,
        [AsParameters] PaginationRequest paging,
        [AsParameters] GetTaskByPaperIdFilter filter)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            return Results.Unauthorized();

        var query = new GetTasksByPaperIdQuery(paperId, currentUser.Id, filter, paging);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetTasksPagedResult>(result));
    }
}

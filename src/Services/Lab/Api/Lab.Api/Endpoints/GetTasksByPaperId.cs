using Lab.Api.Constants;
using Lab.Application.Features.TaskDefinition.Queries.GetTasksByPaperId;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;
using Common.Models;

namespace Lab.Api.Endpoints;

public class GetTasksByPaperId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Task.GetTasksByPaperId, HandleAsync)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(GetTasksByPaperId))
            .Produces<ApiGetResponse<GetTasksPagedResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid paperId,
        [AsParameters] PaginationRequest paging,
        [AsParameters] GetTaskByPaperIdFilter filter)
    {
        var query = new GetTasksByPaperIdQuery(paperId, filter, paging);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetTasksPagedResult>(result));
    }
}

using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.TaskDefinition.Queries.GetMyTask;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetMyTask: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Task.GetMyTasks, HandleGetMyTaskAsync)
            .WithTags(ApiRoutes.Task.Tags)
            .WithName(nameof(GetMyTask))
            .Produces<ApiGetResponse<GetTasksPagedResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }
    
    private async Task<ApiGetResponse<GetTasksPagedResult>> HandleGetMyTaskAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [AsParameters] PaginationRequest paging,
        [AsParameters] GetTaskFilter req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var query = new GetMyTaskQuery(currentUser.Id, req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetTasksPagedResult>(result);
    }
}

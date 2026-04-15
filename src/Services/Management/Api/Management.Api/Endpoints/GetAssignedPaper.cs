using BuildingBlocks.Authentication.Extensions;
using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAssignedPaper : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetAssignedPapers, HandleAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetAssignedPaper))
            .Produces<ApiGetResponse<GetAssignedPapersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [AsParameters] GetAssignedPapersFilter filter,
        [AsParameters] PaginationRequest paging)
    {
        var currentUser = httpContext.GetCurrentUser();

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var query = new GetAssignedPapersQuery(userId, paging, filter);
        var result = await sender.Send(query);

        return Results.Ok(new ApiGetResponse<GetAssignedPapersResult>(result));
    }
}

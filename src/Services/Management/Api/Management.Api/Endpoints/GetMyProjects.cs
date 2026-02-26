using BuildingBlocks.Authentication.Extensions;
using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;

namespace Management.Api.Endpoints;

public sealed class GetMyProjects : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetMyProjects, HandleGetMyProjectsAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetMyProjects))
            .Produces<ApiGetResponse<GetProjectsResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleGetMyProjectsAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [AsParameters] PaginationRequest paging,
        [AsParameters] GetMyProjectsFilter req)
    {
        var currentUser = httpContext.GetCurrentUser();

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var query = new GetMyProjectsQuery(userId, paging, req);
        var result = await sender.Send(query);

        return Results.Ok(new ApiGetResponse<GetProjectsResult>(result));
    }

    #endregion
}




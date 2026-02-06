using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;

namespace Management.Api.Endpoints;

public sealed class GetProjects : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetProjects, HandleGetProjectsAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetProjects))
            .Produces<ApiGetResponse<GetProjectsResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion
    
    #region Methods
    private async Task<ApiGetResponse<GetProjectsResult>> HandleGetProjectsAsync(
        ISender sender,
        [AsParameters] GetProjectsFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetProjectsQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetProjectsResult>(result);
    }
    
    #endregion
}
using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetSubProjects : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.ProjectPaper.GetSubProjects, HandleAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(GetSubProjects))
            .Produces<ApiGetResponse<GetSubProjectsPapersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [AsParameters] PaginationRequest paging,
        [FromQuery] string? title = null)
    {
        var query = new GetSubProjectsQuery(
            ProjectId: projectId,
            Paging: paging,
            Title: title);

        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetSubProjectsPapersResult>(result));
    }

    #endregion
}
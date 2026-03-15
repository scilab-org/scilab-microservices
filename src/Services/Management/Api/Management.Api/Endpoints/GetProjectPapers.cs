using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetProjectPapers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.ProjectPaper.GetProjectPapers, HandleAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(GetProjectPapers))
            .Produces<ApiGetResponse<GetProjectPapersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [AsParameters] GetPaperBanksFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetProjectPapersQuery(projectId, paging, req);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetProjectPapersResult>(result));
    }

    #endregion
}

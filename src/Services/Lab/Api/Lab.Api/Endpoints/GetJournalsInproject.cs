using Lab.Api.Constants;
using Lab.Application.Features.Journal.Queries.GetJournals;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetJournalsInProject : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Journal.GetJournalsInProject, HandleGetJournalsAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(GetJournalsInProject))
            .Produces<ApiGetResponse<GetJournalsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiGetResponse<GetJournalsResult>> HandleGetJournalsAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [AsParameters] GetJournalsFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetJournalsInProjectQuery(req, paging, projectId);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetJournalsResult>(result);
    }

    #endregion
}
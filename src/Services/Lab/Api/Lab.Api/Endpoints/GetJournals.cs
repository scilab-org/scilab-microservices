using Lab.Api.Constants;
using Lab.Application.Features.Journal.Queries.GetJournals;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Common.Models;

namespace Lab.Api.Endpoints;

public class GetJournals : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Journal.GetJournals, HandleGetJournalsAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(GetJournals))
            .Produces<ApiGetResponse<GetJournalsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiGetResponse<GetJournalsResult>> HandleGetJournalsAsync(
        ISender sender,
        [AsParameters] GetJournalsFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetJournalsQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetJournalsResult>(result);
    }

    #endregion
}
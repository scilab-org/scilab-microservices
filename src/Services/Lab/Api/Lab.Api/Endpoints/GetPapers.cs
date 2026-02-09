using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetPapers;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetPapers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetPapers, HandleGetPapersAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPapers))
            .Produces<ApiGetResponse<GetPapersResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiGetResponse<GetPapersResult>> HandleGetPapersAsync(
        ISender sender,
        [AsParameters] GetPapersFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetPapersQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPapersResult>(result);
    }

    #endregion
}
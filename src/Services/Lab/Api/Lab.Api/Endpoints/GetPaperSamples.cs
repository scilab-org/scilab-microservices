using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetPapers;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetPaperSamples : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetPaperSamples, HandleGetPaperSamplesAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperSamples))
            .Produces<ApiGetResponse<GetPapersResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
    
    #region Methods
    private async Task<ApiGetResponse<GetPapersResult>> HandleGetPaperSamplesAsync(
        ISender sender,
        [AsParameters] GetPaperSamplesFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetPaperSamplesQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPapersResult>(result);
    }

    #endregion
}
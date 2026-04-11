using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetCombinePaperById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetCombinePaperById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetCombine, HandleGetCombinePaperByIdQueryAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetCombinePaperById))
            .Produces<ApiGetResponse<CombineSectionsToPaperResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    private async Task<ApiGetResponse<CombineSectionsToPaperResult>> HandleGetCombinePaperByIdQueryAsync(
        ISender sender,
        [FromRoute] Guid paperId,
        [FromQuery] Guid versionId)
    {
        var query = new GetCombinePaperByIdQuery(paperId, versionId);
        var result = await sender.Send(query);

        return new ApiGetResponse<CombineSectionsToPaperResult>(result);
    }
}
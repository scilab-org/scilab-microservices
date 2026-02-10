using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetPaperById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetPaperById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetPaperById, HandleGetPaperByIdAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperById))
            .Produces<ApiGetResponse<GetPaperByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .ProducesProblem(StatusCodes.Status404NotFound)
        // .RequireAuthorization();
    }
    private async Task<ApiGetResponse<GetPaperByIdResult>> HandleGetPaperByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetPaperByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPaperByIdResult>(result);
    }
}
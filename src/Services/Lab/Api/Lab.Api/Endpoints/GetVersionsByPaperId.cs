using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetVersionsByPaperId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetVersionsByPaperId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetVersionsByPaperId, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetVersionsByPaperId))
            .Produces<ApiGetResponse<GetVersionsByPaperIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetVersionsByPaperIdQuery(id);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetVersionsByPaperIdResult>(result));
    }
}
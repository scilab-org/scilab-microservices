using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.GapType.Queries.GetGapTypeById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetGapTypeById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.GapType.GetGapTypeById, HandleGetGapTypeByIdAsync)
            .WithTags(ApiRoutes.GapType.Tags)
            .WithName(nameof(GetGapTypeById))
            .Produces<ApiGetResponse<GetGapTypeByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetGapTypeByIdResult>> HandleGetGapTypeByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetGapTypeByIdQuery(id));
        return new ApiGetResponse<GetGapTypeByIdResult>(result);
    }
}

using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.GapType.Queries.GetGapTypes;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetGapTypes : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.GapType.GetGapTypes, HandleGetGapTypesAsync)
            .WithTags(ApiRoutes.GapType.Tags)
            .WithName(nameof(GetGapTypes))
            .Produces<ApiGetResponse<GetGapTypesResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetGapTypesResult>> HandleGetGapTypesAsync(
        ISender sender,
        [FromQuery] string? name,
        [AsParameters] PaginationRequest paging)
    {
        var result = await sender.Send(new GetGapTypesQuery(paging, name));
        return new ApiGetResponse<GetGapTypesResult>(result);
    }
}

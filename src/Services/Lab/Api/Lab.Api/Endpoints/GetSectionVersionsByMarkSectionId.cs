using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetSectionVersionsByMarkSectionId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSectionVersionsByMarkSectionId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.GetSectionVersionsByMarkSectionId, HandleAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetSectionVersionsByMarkSectionId))
            .Produces<ApiGetResponse<GetSectionVersionsByMarkSectionIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetSectionVersionsByMarkSectionIdQuery(id);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetSectionVersionsByMarkSectionIdResult>(result));
    }
}

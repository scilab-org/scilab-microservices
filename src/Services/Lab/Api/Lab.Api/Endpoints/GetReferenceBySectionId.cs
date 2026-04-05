using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetReferenceBySectionId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetReferenceBySectionId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.Reference, HandleGetReferenceBySectionIdAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetReferenceBySectionId))
            .Produces<ApiGetResponse<GetRefrerenceBySectionIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleGetReferenceBySectionIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetReferenceBySectionIdQuery(id);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetRefrerenceBySectionIdResult>(result));
    }
}
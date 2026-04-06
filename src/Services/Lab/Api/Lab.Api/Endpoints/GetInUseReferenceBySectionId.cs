using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetInUseReferenceBySectionId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetInUseReferenceBySectionId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.InUseReference, HandleGetInUseReferenceBySectionIdAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetInUseReferenceBySectionId))
            .Produces<ApiGetResponse<GetInUseReferenceBySectionIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleGetInUseReferenceBySectionIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetInUseReferenceBySectionIdQuery(id);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetInUseReferenceBySectionIdResult>(result));
    }
}
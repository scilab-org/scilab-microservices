using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetSectionByMarkSectionId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSectionByMarkSectionId: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.GetSectionByMarkSectionId, HandleAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetSectionByMarkSectionId))
            .Produces<ApiGetResponse<GetSectionByMarkSectionIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }
    
    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetSectionByMarkSectionIdQuery(id);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetSectionByMarkSectionIdResult>(result));
    }
}

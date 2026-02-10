using Lab.Api.Constants;
using Lab.Application.Features.Tag.Queries.GetTagById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetTagById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Tag.GetTagById, HandleGetTagByIdAsync)
            .WithTags(ApiRoutes.Tag.Tags)
            .WithName(nameof(GetTagById))
            .Produces<ApiGetResponse<GetTagByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .ProducesProblem(StatusCodes.Status404NotFound)
        // .RequireAuthorization();
    }
    private async Task<ApiGetResponse<GetTagByIdResult>> HandleGetTagByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetTagByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetTagByIdResult>(result);
    }
}
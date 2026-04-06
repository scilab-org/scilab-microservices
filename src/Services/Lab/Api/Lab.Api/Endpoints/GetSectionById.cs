using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetSectionById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSectionById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.GetSectionById, HandleAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetSectionById))
            .Produces<ApiGetResponse<GetSectionByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    private async Task<ApiGetResponse<GetSectionByIdResult>> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetSectionByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetSectionByIdResult>(result);
    }
}
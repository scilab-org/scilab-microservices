using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetSectionnFileById;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSectionFileById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.GetSectionFileById, HandleGetSectionFileByIdAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetSectionFileById))
            .Produces<ApiGetResponse<List<string>>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    private async Task<ApiGetResponse<List<string>>> HandleGetSectionFileByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetSectionnFileByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<List<string>>(result);
    }
}
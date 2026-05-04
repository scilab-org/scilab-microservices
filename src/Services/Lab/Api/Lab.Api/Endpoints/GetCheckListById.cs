using Lab.Api.Constants;
using Lab.Application.Features.CheckList.Queries.GetCheckListById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetCheckListById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.CheckList.GetCheckListById, HandleGetCheckListByIdAsync)
            .WithTags(ApiRoutes.CheckList.Tags)
            .WithName(nameof(GetCheckListById))
            .Produces<ApiGetResponse<GetCheckListByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private async Task<ApiGetResponse<GetCheckListByIdResult>> HandleGetCheckListByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetCheckListByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetCheckListByIdResult>(result);
    }
}

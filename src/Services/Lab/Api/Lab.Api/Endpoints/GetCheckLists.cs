using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.CheckList.Queries.GetCheckLists;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public sealed class GetCheckLists : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.CheckList.GetCheckLists, HandleGetCheckListsAsync)
            .WithTags(ApiRoutes.CheckList.Tags)
            .WithName(nameof(GetCheckLists))
            .Produces<ApiGetResponse<GetCheckListsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetCheckListsResult>> HandleGetCheckListsAsync(
        ISender sender,
        [AsParameters] GetCheckListsFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetCheckListsQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetCheckListsResult>(result);
    }
}

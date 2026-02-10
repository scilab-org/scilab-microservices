using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Tag.Queries.GetTags;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetTags : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Tag.GetTags, HandleGetTagsAsync)
            .WithTags(ApiRoutes.Tag.Tags)
            .WithName(nameof(GetTags))
            .Produces<ApiGetResponse<GetTagsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiGetResponse<GetTagsResult>> HandleGetTagsAsync(
        ISender sender,
        [AsParameters] GetTagsFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetTagsQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetTagsResult>(result);
    }

    #endregion
}
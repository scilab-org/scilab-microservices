using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Template.Queries.GetTemplates;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetTemplates : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Template.GetTemplates, HandleGetTemplatesAsync)
            .WithTags(ApiRoutes.Template.Tags)
            .WithName(nameof(GetTemplates))
            .Produces<ApiGetResponse<GetTemplatesResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetTemplatesResult>> HandleGetTemplatesAsync(
        ISender sender,
        [AsParameters] GetTemplatesFilter filter,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetTemplatesQuery(filter, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetTemplatesResult>(result);
    }

    #endregion
}
using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Dataset.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetDatasets : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Dataset.GetDatasets, HandleGetDatasetsAsync)
            .WithTags(ApiRoutes.Dataset.Tags)
            .WithName(nameof(GetDatasets))
            .Produces<ApiGetResponse<GetDatasetsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .Produces(StatusCodes.Status403Forbidden)
        // .RequireAuthorization();
    }

    #endregion
    
    #region Methods
    
    private async Task<ApiGetResponse<GetDatasetsResult>> HandleGetDatasetsAsync(
        ISender sender,
        [FromQuery] Guid? projectId,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetDatasetsQuery(projectId, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetDatasetsResult>(result);
    }
    
    #endregion
}
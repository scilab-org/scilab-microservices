using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAvailableProjectPapers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.ProjectPaper.GetAvailablePapers, HandleGetAvailableProjectPapersAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(GetAvailableProjectPapers))
            .Produces<ApiGetResponse<GetAvailablePapersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleGetAvailableProjectPapersAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromQuery] string? searchText)
    {
        var query = new GetAvailablePapersQuery(
            ProjectId: projectId,
            SearchText: searchText);

        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetAvailablePapersResult>(result));
    }

    #endregion
}

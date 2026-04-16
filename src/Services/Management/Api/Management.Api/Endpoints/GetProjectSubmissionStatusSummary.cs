using Common.Models;
using Management.Api.Constants;
using Management.Application.Dtos.Papers;
using Management.Application.Features.Project.Queries;

namespace Management.Api.Endpoints;

public class GetProjectSubmissionStatusSummary : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.ProjectPaper.GetSubmissionStatusSummary, HandleAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(GetProjectSubmissionStatusSummary))
            .Produces<ApiGetResponse<SubmissionStatusSummaryResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(ISender sender, Guid projectId)
    {
        var query = new GetProjectSubmissionStatusSummaryQuery(projectId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<SubmissionStatusSummaryResult>(result));
    }

    #endregion
}

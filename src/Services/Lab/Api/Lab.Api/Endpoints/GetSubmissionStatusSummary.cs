using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetSubmissionStatusSummary;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSubmissionStatusSummary : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.GetSubmissionStatusSummary, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetSubmissionStatusSummary))
            .Produces<GetSubmissionStatusSummaryResult>(StatusCodes.Status200OK)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromBody] GetSubmissionStatusSummaryRequest dto)
    {
        var query = new GetSubmissionStatusSummaryQuery(dto.PaperIds);
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    #endregion
}

using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetPaperContributors;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetPaperContributors : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperContributor.GetPaperContributors, HandleAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(GetPaperContributors))
            .Produces<ApiGetResponse<GetPaperContributorsResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        Guid paperId)
    {
        var query = new GetPaperContributorsQuery(paperId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetPaperContributorsResult>(result));
    }

    #endregion
}


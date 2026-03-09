using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetSectionsByPaperId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetSectionsByPaperId : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetSectionsByPaperId, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetSectionsByPaperId))
            .Produces<ApiGetResponse<GetSectionsByPaperIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetSectionsByPaperIdQuery(id);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetSectionsByPaperIdResult>(result));
    }

    #endregion
}



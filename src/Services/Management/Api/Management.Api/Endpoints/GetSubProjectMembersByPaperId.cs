using Management.Api.Constants;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetSubProjectMembersByPaperId : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.SubProject.GetSubProjectMembersByPaperId, HandleAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(GetSubProjectMembersByPaperId))
            .Produces<ApiGetResponse<GetSubProjectMembersByPaperIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid paperId)
    {
        var query = new GetSubProjectMembersByPaperIdQuery(paperId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetSubProjectMembersByPaperIdResult>(result));
    }

    #endregion
}


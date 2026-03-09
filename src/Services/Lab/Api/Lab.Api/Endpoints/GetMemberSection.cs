using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetMemberSection;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetMemberSection : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperContributor.GetMemberSection, HandleAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(GetMemberSection))
            .Produces<ApiGetResponse<GetMemberSectionResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid sectionId,
        [FromQuery] Guid paperId)
    {
        var query = new GetMemberSectionQuery(sectionId, paperId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetMemberSectionResult>(result));
    }

    #endregion
}

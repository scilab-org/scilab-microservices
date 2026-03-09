using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetAvailableMemberSection;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetAvailableMemberSection : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperContributor.GetAvailableMemberSection, HandleAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(GetAvailableMemberSection))
            .Produces<ApiGetResponse<GetAvailableMemberSectionResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid sectionId,
        [FromQuery] Guid paperId)
    {
        var query = new GetAvailableMemberSectionQuery(sectionId, paperId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<GetAvailableMemberSectionResult>(result));
    }

    #endregion
}


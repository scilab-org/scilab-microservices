using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetMemberByPaperId : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.SubProject.GetMemberByPaperId, HandleAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(GetMemberByPaperId))
            .Produces<ApiGetResponse<ProjectMemberDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid paperId,
        [FromQuery] Guid userId)
    {
        var query = new GetMemberByPaperIdQuery(paperId, userId);
        var result = await sender.Send(query);
        return TypedResults.Ok(new ApiGetResponse<ProjectMemberDto>(result));
    }

    #endregion
}



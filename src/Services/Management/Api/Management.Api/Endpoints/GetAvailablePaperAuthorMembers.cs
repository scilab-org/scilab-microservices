using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAvailablePaperAuthorMembers : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.SubProject.GetAvailablePaperAuthorMembers, HandleAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(GetAvailablePaperAuthorMembers))
            .Produces<ApiGetResponse<GetProjectMembersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid subProjectId,
        [FromQuery] Guid paperId,
        [AsParameters] PaginationRequest paging)
    {
        var result = await sender.Send(new GetAvailablePaperAuthorMembersQuery(subProjectId, paperId, paging));
        return TypedResults.Ok(new ApiGetResponse<GetProjectMembersResult>(result));
    }
}

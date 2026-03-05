using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetAvailableSubProjectMembers: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.SubProject.GetAvailableSubProjectMembers, HandleGetAvailableSubProjectMembersAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(GetAvailableSubProjectMembers))
            .Produces<ApiGetResponse<GetAvailableProjectUsersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleGetAvailableSubProjectMembersAsync(
        ISender sender,
        [FromRoute] Guid subProjectId,
        [AsParameters] GetAvailableSubProjectMembersFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetAvailableSubProjectMembersQuery(subProjectId, req, paging);

        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetProjectMembersResult>(result));
    }

    #endregion
}

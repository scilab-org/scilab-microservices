using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetSubProjectMembers: ICarterModule
{
#region Implementations

public void AddRoutes(IEndpointRouteBuilder app)
{
    app.MapGet(ApiRoutes.SubProject.GetSubProjectMembers, HandleGetSubProjectMembersAsync)
        .WithTags(ApiRoutes.SubProject.Tags)
        .WithName(nameof(GetSubProjectMembers))
        .Produces<ApiGetResponse<GetProjectMembersResult>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    // .RequireAuthorization();
}

#endregion

#region Methods

private async Task<IResult> HandleGetSubProjectMembersAsync(
    ISender sender,
    [FromRoute] Guid subProjectId,
    [AsParameters] GetProjectMembersFilter req,
    [AsParameters] PaginationRequest paging)
{
        
    var query = new GetSubProjectMembersQuery(subProjectId, req, paging);

    var result = await sender.Send(query);

    return TypedResults.Ok(new ApiGetResponse<GetProjectMembersResult>(result));
}

#endregion
}
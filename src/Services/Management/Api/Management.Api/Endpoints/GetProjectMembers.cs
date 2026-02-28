using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Member.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetProjectMembers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Member.GetProjectMembers, HandleGetProjectMembersAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(GetProjectMembers))
            .Produces<ApiGetResponse<GetProjectMembersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleGetProjectMembersAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromQuery] string? searchEmail,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var paging = new PaginationRequest(pageNumber, pageSize);
        var query = new GetProjectMembersQuery(projectId, searchEmail, paging);

        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetProjectMembersResult>(result));
    }

    #endregion
}



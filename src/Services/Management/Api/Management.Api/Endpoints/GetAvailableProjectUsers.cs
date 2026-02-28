using Common.Constants;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAvailableProjectUsers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetAvailableProjectUsers, HandleGetAvailableProjectUsersAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetAvailableProjectUsers))
            .Produces<ApiGetResponse<GetAvailableProjectUsersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleGetAvailableProjectUsersAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        [FromQuery] string? searchText,
        [FromQuery] string adminGroupName = AuthorizeConstants.SystemAdmin)
    {
        var query = new GetAvailableProjectUsersQuery(
            ProjectId: projectId,
            AdminGroupName: adminGroupName,
            SearchText: searchText);

        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetAvailableProjectUsersResult>(result));
    }

    #endregion
}


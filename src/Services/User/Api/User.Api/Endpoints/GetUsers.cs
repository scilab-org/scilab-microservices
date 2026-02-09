#region using

using Common.Models;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Models.Filters;
using User.Application.Models.Results;
using User.Application.Features.Users.Queries;

#endregion

namespace User.Api.Endpoints;

public sealed class GetUsers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Users.GetAll, HandleGetUsersAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(GetUsers))
            .Produces<ApiGetResponse<GetUsersResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetUsersResult>> HandleGetUsersAsync(
        ISender sender,
        [FromQuery] string? searchText,
        [FromQuery] string? groupName,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 1000)
    {
        var filter = new GetUsersFilter(searchText, groupName);
        var paging = new PaginationRequest(pageNumber, pageSize);
        var query = new GetUsersQuery(filter, paging);

        var result = await sender.Send(query);

        return new ApiGetResponse<GetUsersResult>(result);
    }

    #endregion
}

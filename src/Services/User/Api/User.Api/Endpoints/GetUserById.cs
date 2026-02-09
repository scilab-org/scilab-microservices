#region using

using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Features.Users.Queries;
using User.Application.Models.Results;

#endregion

namespace User.Api.Endpoints;

public sealed class GetUserById : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Users.GetById, HandleGetUserByIdAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(GetUserById))
            .Produces<ApiGetResponse<GetUserByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetUserByIdResult>> HandleGetUserByIdAsync(
        ISender sender,
        [FromRoute] string userId)
    {
        var query = new GetUserByIdQuery(userId);

        var result = await sender.Send(query);

        return new ApiGetResponse<GetUserByIdResult>(result);
    }

    #endregion
}

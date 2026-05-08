using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Features.Dashboard;

namespace Management.Api.Endpoints;

public sealed class GetUserDashboard : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Dashboard.GetUserDashboard, HandleAsync)
            .WithTags(ApiRoutes.Dashboard.Tags)
            .WithName(nameof(GetUserDashboard))
            .Produces<UserDashboardResult>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        CancellationToken ct)
    {
        var currentUser = httpContext.GetCurrentUser();
        var query = new GetUserDashboardQuery(Guid.Parse(currentUser.Id), currentUser.UserName);
        var result = await sender.Send(query, ct);
        return TypedResults.Ok(result);
    }

    #endregion
}

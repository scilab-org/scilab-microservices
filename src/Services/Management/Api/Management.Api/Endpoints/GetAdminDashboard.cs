using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using Management.Api.Constants;
using Management.Application.Features.Dashboard;

namespace Management.Api.Endpoints;

public sealed class GetAdminDashboard : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Dashboard.GetAdminDashboard, HandleAsync)
            .WithTags(ApiRoutes.Dashboard.Tags)
            .WithName(nameof(GetAdminDashboard))
            .Produces<AdminDashboardResult>(StatusCodes.Status200OK)
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
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin))
            throw new UnauthorizedAccessException();

        var result = await sender.Send(new GetAdminDashboardQuery(), ct);
        return TypedResults.Ok(result);
    }

    #endregion
}

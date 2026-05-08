using Lab.Api.Constants;
using Lab.Application.Features.Dashboard;

namespace Lab.Api.Endpoints;

public sealed class GetUserDashboardKpis : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Dashboard.GetUserKpis, HandleAsync)
            .WithTags(ApiRoutes.Dashboard.Tags)
            .WithName(nameof(GetUserDashboardKpis))
            .Produces<UserDashboardKpisResult>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [Microsoft.AspNetCore.Mvc.FromQuery] string username,
        [Microsoft.AspNetCore.Mvc.FromQuery] Guid[]? memberIds,
        CancellationToken ct)
    {
        var query = new GetUserDashboardKpisQuery(username, memberIds ?? []);
        var result = await sender.Send(query, ct);
        return TypedResults.Ok(result);
    }

    #endregion
}

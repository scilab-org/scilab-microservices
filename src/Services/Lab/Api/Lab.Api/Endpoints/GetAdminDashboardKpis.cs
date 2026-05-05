using Lab.Api.Constants;
using Lab.Application.Features.Dashboard;

namespace Lab.Api.Endpoints;

public sealed class GetAdminDashboardKpis : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Dashboard.GetAdminKpis, HandleAsync)
            .WithTags(ApiRoutes.Dashboard.Tags)
            .WithName(nameof(GetAdminDashboardKpis))
            .Produces<AdminDashboardKpisResult>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new GetAdminDashboardKpisQuery(), ct);
        return TypedResults.Ok(result);
    }

    #endregion
}

using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetMyProjectRole: ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetMyProjectRole, HandleGetMyProjectRoleAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetMyProjectRole))
            .Produces<ApiGetResponse<string>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
    #endregion
    
    #region Methods

    private async Task<IResult> HandleGetMyProjectRoleAsync(
        ISender sender,
        [FromRoute] Guid projectId,
        IHttpContextAccessor httpContext)
    {
        var currentUser = httpContext.GetCurrentUser();

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var query = new GetMyProjectRoleQuery(userId, projectId);
        var result = await sender.Send(query);

        return Results.Ok(new ApiGetResponse<string>(result));
    }

    #endregion
}
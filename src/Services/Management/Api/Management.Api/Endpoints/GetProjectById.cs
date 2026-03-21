using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class GetProjectById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetProjectById, HandleGetProjectByIdAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetProjectById))
            .Produces<ApiGetResponse<GetProjectByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
    private async Task<ApiGetResponse<GetProjectByIdResult>> HandleGetProjectByIdAsync(
        ISender sender, 
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            throw new UnauthorizedAccessException();
        
        
        
        var query = new GetProjectByIdQuery(projectId, userId, currentUser.Groups!);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetProjectByIdResult>(result);
    }
}
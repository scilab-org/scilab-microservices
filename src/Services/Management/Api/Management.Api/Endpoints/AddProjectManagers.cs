using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class AddProjectManagers : ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Member.AddProjectManagers, HandleAddProjectManagersAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(AddProjectManagers))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
    #endregion
    
    #region Methods

    private async Task<IResult> HandleAddProjectManagersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId,
        [FromBody] AddProjectManagersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        if (currentUser.Groups == null ||
            !currentUser.Groups.Any(g => g.Equals(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new AddProjectManagersCommand(projectId, req);

        var result = await sender.Send(command);

        return TypedResults.Created(
            $"/projects/{projectId}/managers",
            new ApiCreatedResponse<List<Guid>>(result));
    }
    
    #endregion
}

using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteProjectManagers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Member.DeleteProjectManagers, HandleDeleteProjectManagersAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(DeleteProjectManagers))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteProjectManagersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId,
        [FromBody] DeleteProjectManagersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        if (currentUser.Groups == null ||
            !currentUser.Groups.Any(g => g.Equals(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new DeleteProjectManagersCommand(projectId, req);
        var result = await sender.Send(command);
        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }

    #endregion
}


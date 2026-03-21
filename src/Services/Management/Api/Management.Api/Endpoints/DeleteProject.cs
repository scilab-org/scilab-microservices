using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Management.Api.Constants;
using Management.Application.Features.Project.Commands;

namespace Management.Api.Endpoints;

public class DeleteProject : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Project.Delete, HandleDeleteProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(DeleteProject))
            .Produces<ApiDeletedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteProjectAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        Guid projectId)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            throw new UnauthorizedException(MessageCode.Unauthorized);
        if (currentUser.Groups == null ||
            !currentUser.Groups.Any(g => g.Equals(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new DeleteProjectCommand(projectId);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(projectId);
    }

    #endregion
}
using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class UpdateProject : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Project.Update, HandleUpdateProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(UpdateProject))
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion
    #region Methods
    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateProjectAsync(
        ISender sender,
        Guid projectId,
        IHttpContextAccessor httpContext,
        [FromBody] UpdateProjectDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            throw new UnauthorizedException(MessageCode.Unauthorized);
        if (currentUser.Groups == null ||
            !currentUser.Groups.Any(g => g.Equals(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
            throw new NoPermissionException(MessageCode.AccessDenied);

        var command = new UpdateProjectCommand(projectId, req);

        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
    #endregion
}
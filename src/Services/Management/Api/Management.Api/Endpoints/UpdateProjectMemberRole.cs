using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class UpdateProjectMemberRole : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Member.UpdateProjectMemberRole, HandleUpdateProjectMemberRoleAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(UpdateProjectMemberRole))
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleUpdateProjectMemberRoleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId,
        [FromRoute] Guid memberId,
        [FromBody] string projectRole)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        var dto = new UpdateProjectMemberRoleDto { MemberId = memberId, ProjectRole = projectRole };
        var command = new UpdateProjectMemberRoleCommand(projectId, dto, currentUser.Id);
        var result = await sender.Send(command);
        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }

    #endregion
}


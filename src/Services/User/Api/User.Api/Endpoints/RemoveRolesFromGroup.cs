#region using

using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Features.Roles.Commands;

#endregion

namespace User.Api.Endpoints;

public sealed class RemoveRolesFromGroup : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Roles.RemoveFromGroup, HandleRemoveRolesFromGroupAsync)
            .WithTags(ApiRoutes.Groups.Tags)
            .WithName(nameof(RemoveRolesFromGroup))
            .Produces<ApiDeletedResponse<bool>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<bool>> HandleRemoveRolesFromGroupAsync(
        ISender sender,
        [FromRoute] string groupId,
        [FromBody] List<string> roleNames)
    {
        var command = new RemoveRolesFromGroupCommand(groupId, roleNames);

        var result = await sender.Send(command);

        return new ApiDeletedResponse<bool>(result);
    }

    #endregion
}

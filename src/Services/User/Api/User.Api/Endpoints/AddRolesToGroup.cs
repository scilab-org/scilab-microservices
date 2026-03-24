#region using

using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Application.Features.Roles.Commands;

#endregion

namespace User.Api.Endpoints;

public sealed class AddRolesToGroup : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Roles.AddToGroup, HandleAddRolesToGroupAsync)
            .WithTags(ApiRoutes.Groups.Tags)
            .WithName(nameof(AddRolesToGroup))
            .Produces<ApiUpdatedResponse<bool>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<bool>> HandleAddRolesToGroupAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string groupId,
        [FromBody] List<string> roleNames)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin))
        {
            throw new UnauthorizedAccessException();
        }
        
        var command = new AddRolesToGroupCommand(groupId, roleNames);

        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);
    }

    #endregion
}

#region using

using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using User.Api.Constants;
using User.Api.Models;
using User.Application.Dtos.Users;
using User.Application.Features.Users;

#endregion

namespace User.Api.Endpoints;

public sealed class UpdateUser : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Users.Update, HandleUpdateUserAsync)
            .WithTags(ApiRoutes.Users.Tags)
            .WithName(nameof(UpdateUser))
            .WithMultipartForm<UpdateUserRequest>()
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization("admin");
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<bool>> HandleUpdateUserAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] string userId,
        [FromForm] UpdateUserRequest req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (!currentUser.HasGroups(AuthorizeConstants.SystemAdmin) &&
            !currentUser.HasGroups(AuthorizeConstants.ProjectManager) &&
            !currentUser.HasGroups(AuthorizeConstants.ProjectAuthor))
        {
            throw new UnauthorizedAccessException();
        }

        var dto = new UpdateUserDto
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Enabled = req.Enabled,
            GroupNames = string.IsNullOrWhiteSpace(req.GroupNames)
                ? null
                : req.GroupNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };

        if (req.AvatarImage is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await req.AvatarImage.CopyToAsync(ms);
            dto.AvatarImage = new UploadFileBytes
            {
                FileName = req.AvatarImage.FileName,
                ContentType = req.AvatarImage.ContentType,
                Bytes = ms.ToArray()
            };
        }

        var command = new UpdateUserCommand(userId, dto, Actor.User(currentUser.Email));
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<bool>(result);
    }

    #endregion
}

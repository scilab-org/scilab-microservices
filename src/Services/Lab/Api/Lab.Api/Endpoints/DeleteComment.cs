using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Features.Comment.Commands.DeleteComment;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeleteComment: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Comment.Delete, HandleDeleteComment)
            .WithTags(ApiRoutes.Comment.Tags)
            .WithName(nameof(DeleteComment))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteComment(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var command = new DeleteCommentCommand(id, currentUser.UserName);
        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
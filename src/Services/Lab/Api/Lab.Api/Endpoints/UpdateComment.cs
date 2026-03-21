using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Features.Comment.Commands.UpdateComment;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateComment: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Comment.Update, HandleUpdateCommentAsync)
            .WithTags(ApiRoutes.Comment.Tags)
            .WithName(nameof(UpdateComment))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateCommentAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] string content)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            throw new UnauthorizedAccessException();
        
        var command = new UpdateCommentCommand(id, content, currentUser.UserName);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
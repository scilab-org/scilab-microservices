using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Features.CheckList.Commands.DeleteCheckList;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class DeleteCheckList : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.CheckList.Delete, HandleDeleteCheckListAsync)
            .WithTags(ApiRoutes.CheckList.Tags)
            .WithName(nameof(DeleteCheckList))
            .Produces<ApiDeletedResponse<Unit>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<IResult> HandleDeleteCheckListAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new DeleteCheckListCommand(id, currentUser.UserName);
        await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<Unit>());
    }
}

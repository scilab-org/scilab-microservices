using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.UpdateCombinePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateCombinePaper : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Paper.GetVersionById, HandleUpdateCombinePaperByIdQueryAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(UpdateCombinePaper))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateCombinePaperByIdQueryAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid paperId,
        [FromRoute] Guid versionId,
        [FromBody] UpdateCombinePaperDto request)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new UpdateCombinePaperCommand(paperId, versionId, currentUser.UserName, request);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
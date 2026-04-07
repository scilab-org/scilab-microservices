using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.UpdateGuideline;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateSectionGuideline: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.UpdateGuideline, HandleUpdateGuidelineAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(UpdateSectionGuideline))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateGuidelineAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] UpdateGuidelineDto request)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new UpdateGuidelineCommand(request, id, userId, currentUser.UserName);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<Guid>(result);
    }
}
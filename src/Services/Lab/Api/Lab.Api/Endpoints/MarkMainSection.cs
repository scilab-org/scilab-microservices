using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.MarkMainSection;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class MarkMainSection : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.MarkMainSection, HandleMarkMainSectionAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(MarkMainSection))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleMarkMainSectionAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] MarkMainSectionDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);
        var command = new MarkMainSectionCommand(dto, id);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<Guid>(result);
    }
}
using AutoMapper;
using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.UpsertSection;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateSection : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.Update, HandleUpdateSectionAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(UpdateSection))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateSectionAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        IMapper mapper,
        [FromRoute] Guid id,
        [FromBody] UpsertSectionDto request)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var command = new UpsertSectionCommand(request, id, currentUser.UserName);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<Guid>(result);
    }
}
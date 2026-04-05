using BuildingBlocks.Authentication.Extensions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Commands.UpdateReference;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateReference : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.Reference, HandleUpdateSectionAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(UpdateReference))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleUpdateSectionAsync(
        ISender sender,
        Guid id,
        IHttpContextAccessor accessor,
        [FromBody] UpdateReferenceDto dto)
    {
        var currentUser = accessor.GetCurrentUser()
                          ?? throw new UnauthorizedAccessException(MessageCode.Unauthorized);

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var command = new UpdateReferenceCommand(dto, userId, currentUser.UserName, id);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }

    #endregion
}
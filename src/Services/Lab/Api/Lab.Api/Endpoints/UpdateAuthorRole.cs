using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.AuthorRoles;
using Lab.Application.Features.AuthorRole.Commands.UpdateAuthorRole;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdateAuthorRole : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.AuthorRole.Update, HandleUpdateAuthorRoleAsync)
            .WithTags(ApiRoutes.AuthorRole.Tags)
            .WithName(nameof(UpdateAuthorRole))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateAuthorRoleAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateAuthorRoleDto dto)
    {
        var command = new UpdateAuthorRoleCommand(id, dto);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<Guid>(result);
    }
}

using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.AuthorRoles;
using Lab.Application.Features.AuthorRole.Commands.CreateAuthorRole;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateAuthorRole : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.AuthorRole.Create, HandleCreateAuthorRoleAsync)
            .WithTags(ApiRoutes.AuthorRole.Tags)
            .WithName(nameof(CreateAuthorRole))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<IResult> HandleCreateAuthorRoleAsync(
        ISender sender, 
        [FromBody] CreateAuthorRoleDto dto)
    {
        var command = new CreateAuthorRoleCommand(dto);
        var result = await sender.Send(command);
        return TypedResults.Created($"{ApiRoutes.AuthorRole.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

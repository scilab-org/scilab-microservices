using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.AuthorRole.Commands.DeleteAuthorRole;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class DeleteAuthorRole : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.AuthorRole.Delete, HandleDeleteAuthorRoleAsync)
            .WithTags(ApiRoutes.AuthorRole.Tags)
            .WithName(nameof(DeleteAuthorRole))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteAuthorRoleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        await sender.Send(new DeleteAuthorRoleCommand(id));
        return new ApiDeletedResponse<Guid>(id);
    }
}

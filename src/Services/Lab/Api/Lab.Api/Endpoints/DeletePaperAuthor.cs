using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperAuthor.Commands.DeletePaperAuthor;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class DeletePaperAuthor : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.PaperAuthor.Delete, HandleDeletePaperAuthorAsync)
            .WithTags(ApiRoutes.PaperAuthor.Tags)
            .WithName(nameof(DeletePaperAuthor))
            .Produces<ApiDeletedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeletePaperAuthorAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        await sender.Send(new DeletePaperAuthorCommand(id));
        return new ApiDeletedResponse<Guid>(id);
    }
}

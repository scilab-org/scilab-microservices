using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.PaperAuthors;
using Lab.Application.Features.PaperAuthor.Commands.UpdatePaperAuthor;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdatePaperAuthor : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.PaperAuthor.Update, HandleUpdatePaperAuthorAsync)
            .WithTags(ApiRoutes.PaperAuthor.Tags)
            .WithName(nameof(UpdatePaperAuthor))
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdatePaperAuthorAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdatePaperAuthorDto dto)
    {
        var command = new UpdatePaperAuthorCommand(id, dto);
        await sender.Send(command);
        return new ApiUpdatedResponse<Guid>( id);
    }
}

using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.GapType.Commands.DeleteGapType;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class DeleteGapType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.GapType.Delete, HandleDeleteGapTypeAsync)
            .WithTags(ApiRoutes.GapType.Tags)
            .WithName(nameof(DeleteGapType))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteGapTypeAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteGapTypeCommand(id));
        return new ApiDeletedResponse<Guid>(result);
    }
}

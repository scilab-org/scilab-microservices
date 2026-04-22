using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.GapTypes;
using Lab.Application.Features.GapType.Commands.UpdateGapType;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdateGapType : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.GapType.Update, HandleUpdateGapTypeAsync)
            .WithTags(ApiRoutes.GapType.Tags)
            .WithName(nameof(UpdateGapType))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateGapTypeAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateGapTypeDto dto)
    {
        var result = await sender.Send(new UpdateGapTypeCommand(id, dto));
        return new ApiUpdatedResponse<Guid>(result);
    }
}

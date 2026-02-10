using Lab.Api.Constants;
using Lab.Api.Models.Tag;
using Lab.Application.Features.Tag.Commands.UpdateTag;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateTag : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Tag.Update, HandleUpdateTagAsync)
            .WithTags(ApiRoutes.Tag.Tags)
            .WithName(nameof(UpdateTag))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateTagAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateTagRequest request)
    {
        var command = new UpdateTagCommand(id, request.Name);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
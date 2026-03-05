using AutoMapper;
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
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateSectionAsync(
        ISender sender,
        IMapper mapper,
        [FromRoute] Guid id,
        [FromBody] UpsertSectionDto request)
    {
        var command = new UpsertSectionCommand(request, id);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<Guid>(result);
    }
}
using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Api.Models.Papers;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.UpdatePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdatePaper : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Paper.Update, HandleUpdatePaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(UpdatePaper))
            .WithMultipartForm<UpdatePaperRequest>()
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdatePaperAsync(
        ISender sender,
        IMapper mapper,
        [FromRoute] Guid id,
        [FromForm] UpdatePaperRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = mapper.Map<UpdatePaperDto>(req);

        var command = new UpdatePaperCommand(id, dto);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
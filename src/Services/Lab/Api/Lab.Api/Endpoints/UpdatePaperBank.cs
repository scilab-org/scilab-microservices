using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.PaperBank;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdatePaperBank : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.PaperBank.Update, HandleUpdatePaperBankAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(UpdatePaperBank))
            .WithMultipartForm<UpdatePaperBankRequest>()
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdatePaperBankAsync(
        ISender sender,
        IMapper mapper,
        [FromRoute] Guid id,
        [FromForm] UpdatePaperBankRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = mapper.Map<UpdatePaperBankDto>(req);

        if (req.BibFile != null)
        {
            using var ms = new MemoryStream();
            await req.BibFile.CopyToAsync(ms);
            dto.UploadBibFile = new UploadFileBytes()
            {
                FileName = req.BibFile.FileName,
                ContentType = req.BibFile.ContentType,
                Bytes = ms.ToArray()
            };
        }

        var command = new UpdatePaperBankCommand(id, dto);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}
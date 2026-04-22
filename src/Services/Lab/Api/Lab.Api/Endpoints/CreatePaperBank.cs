using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.PaperBank;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Features.PaperBank.Commands.CreatePaperBank;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreatePaperBank : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.PaperBank.Create, HandleCreatePaperBankAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(CreatePaperBank))
            .WithMultipartForm<CreatePaperBankRequest>()
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreatePaperBankAsync(
        ISender sender,
        IMapper mapper,
        HttpRequest httpRequest,
        [FromForm] CreatePaperBankRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = mapper.Map<CreatePaperBankDto>(req);

        if (req.PdfFile != null)
        {
            using var ms = new MemoryStream();
            await req.PdfFile.CopyToAsync(ms);
            dto.UploadPdfFile = new UploadFileBytes()
            {
                FileName = req.PdfFile.FileName,
                ContentType = req.PdfFile.ContentType,
                Bytes = ms.ToArray()
            };
        }

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

        var command = new CreatePaperBankCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.PaperBank.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
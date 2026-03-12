using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Section.Commands.UploadSectionFile;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UploadSectionFile : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Section.Upload, HandleUploadSectionFileAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(UploadSectionFile))
            .WithMultipartForm<UploadSectionFileRequest>()
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<Guid>> HandleUploadSectionFileAsync(
        ISender sender,
        IMapper mapper,
        [FromRoute] Guid id,
        [FromForm] UploadSectionFileRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = mapper.Map<UploadSectionFileDto>(req);

        using var ms = new MemoryStream();
        await req.File.CopyToAsync(ms);
        dto.UploadFile = new UploadFileBytes()
        {
            FileName = req.File.FileName,
            ContentType = req.File.ContentType,
            Bytes = ms.ToArray()
        };

        var command = new UploadSectionFileCommand(dto, id);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }

    #endregion
}
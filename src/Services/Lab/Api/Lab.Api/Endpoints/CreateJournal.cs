using Common.Models.Reponses;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.Journal;
using Lab.Application.Dtos.Journals;
using Lab.Application.Features.Journal.Commands.CreateJournal;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateJournal : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Journal.Create, HandleCreateJournalAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(CreateJournal))
            .WithMultipartForm<CreateJournalRequest>()
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateJournalAsync(
        ISender sender,
        [FromForm] CreateJournalRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = new CreateJournalEntityDto
        {
            Name = req.Name,
            ProjectId = req.ProjectId,
            StartAt = req.StartAt,
            EndAt = req.EndAt,
            Style = req.Style,
            TexUploadFile = await ToUploadFileAsync(req.TexFile),
            PdfUploadFile = await ToUploadFileAsync(req.PdfFile)
        };

        var command = new CreateJournalCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Journal.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    private static async Task<UploadFileBytes?> ToUploadFileAsync(IFormFile? file)
    {
        if (file == null) return null;

        await using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        return new UploadFileBytes
        {
            FileName = file.FileName,
            ContentType = file.ContentType,
            Bytes = ms.ToArray()
        };
    }

    #endregion
}
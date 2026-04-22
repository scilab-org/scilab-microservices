using BuildingBlocks.Authentication.Extensions;
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
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateJournalAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromForm] CreateJournalRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var dto = new CreateJournalEntityDto
        {
            Name = req.Name,
            TemplateIds = req.TemplateIds,
            ISSN = req.ISSN,
            Ranking = req.Ranking,
            Type = req.Type,
            Url = req.Url,
            Style = req.Style,
            TexUploadFile = await ToUploadFileAsync(req.TexFile),
            PdfUploadFile = await ToUploadFileAsync(req.PdfFile)
        };

        var command = new CreateJournalCommand(dto, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Journal.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    private static async Task<UploadFileBytes?> ToUploadFileAsync(IFormFile? file)
    {
        if (file != null)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            return new UploadFileBytes
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Bytes = ms.ToArray()
            };
        }
        return null;
    }

    #endregion
}
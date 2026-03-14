using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.PaperBank;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Features.PaperBank.Commands.CreatePaperBank;
using Lab.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        var parsedText = await ResolveParsedTextAsync(httpRequest, req.ParsedText);

        var dto = mapper.Map<CreatePaperBankDto>(req);
        dto.ParsedText = parsedText;

        if (req.File != null)
        {
            using var ms = new MemoryStream();
            await req.File.CopyToAsync(ms);
            dto.UploadFile = new UploadFileBytes()
            {
                FileName = req.File.FileName,
                ContentType = req.File.ContentType,
                Bytes = ms.ToArray()
            };
        }

        var command = new CreatePaperBankCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.PaperBank.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion

    #region Helpers

    private static async Task<ParsedText?> ResolveParsedTextAsync(HttpRequest httpRequest, ParsedText? parsedText)
    {
        if (parsedText is not null) return parsedText;
        if (!httpRequest.HasFormContentType) return null;

        var form = await httpRequest.ReadFormAsync();
        if (!form.TryGetValue("parsedText", out var parsedTextValues)) return null;

        var rawParsedText = parsedTextValues.ToString();
        if (string.IsNullOrWhiteSpace(rawParsedText)) return null;

        try
        {
            return JsonSerializer.Deserialize<ParsedText>(
                rawParsedText,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            throw new ClientValidationException(
                MessageCode.BadRequest,
                new
                {
                    Field = "parsedText",
                    Error = "INVALID_JSON"
                });
        }
    }

    #endregion
}
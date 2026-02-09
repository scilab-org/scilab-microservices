using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.Papers;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CreatePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreatePaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.Create, HandleCreatePaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(CreatePaper))
            .WithMultipartForm<CreatePaperRequest>()
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreatePaperAsync(
        ISender sender,
        IMapper mapper,
        [FromForm] CreatePaperRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var dto = mapper.Map<CreatePaperDto>(req);

        if (req.File != null)
        {
            using var ms = new MemoryStream();
            await req.File.CopyToAsync(ms);
            dto.UploadFile ??= new UploadFileBytes()
            {
                FileName = req.File.FileName,
                ContentType = req.File.ContentType,
                Bytes = ms.ToArray()
            };
        }

        var command = new CreatePaperCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Paper.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
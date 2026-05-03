using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Swagger.Extensions;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.UploadPaperFile;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UploadPaperFile : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.UploadPaperFile, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(UploadPaperFile))
            .WithMultipartForm<CreatePaperVersionFileRequest>()
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid paperId,
        [FromForm] CreatePaperVersionFileRequest req)
    {
        var currentUser = httpContext.GetCurrentUser();

        using var ms = new MemoryStream();
        await req.File.CopyToAsync(ms);

        var dto = new CreatePaperVersionFileDto
        {
            UploadFile = new UploadFileBytes
            {
                FileName = req.File.FileName,
                ContentType = req.File.ContentType,
                Bytes = ms.ToArray()
            },
            Note = req.Note
        };

        var command = new UploadPaperFileCommand(paperId, dto, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created(
            $"{ApiRoutes.Paper.GetVersionFileById.Replace("{id}", result.ToString())}",
            new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}

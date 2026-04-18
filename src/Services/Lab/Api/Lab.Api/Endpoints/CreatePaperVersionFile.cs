using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Swagger.Extensions;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.CreatePaperVersionFile;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreatePaperVersionFileRequest
{
    public IFormFile File { get; set; } = null!;
    public string? Note { get; set; }
}

public class CreatePaperVersionFile : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.CreateVersionFile, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(CreatePaperVersionFile))
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
        [FromRoute] Guid versionId,
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

        var command = new CreatePaperVersionFileCommand(paperId, versionId, dto, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created(
            $"{ApiRoutes.Paper.GetVersionFileById.Replace("{id}", result.ToString())}",
            new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}

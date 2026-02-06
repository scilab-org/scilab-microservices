using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Management.Api.Constants;
using Management.Api.Models;
using Management.Application.Dtos.Datasets;
using Management.Application.Features.Dataset.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class CreateDataset : ICarterModule
{
    #region Implementations
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Dataset.Create, HandleCreateDatasetAsync)
            .WithTags(ApiRoutes.Dataset.Tags)
            .WithName(nameof(CreateDataset))
            .WithMultipartForm<CreateDatasetRequest>()
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }
    
    #endregion

    #region Methods
    
    private async Task<ApiCreatedResponse<Guid>> HandleCreateDatasetAsync(
        ISender sender,
        IMapper mapper,
        [FromForm] CreateDatasetRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);
        if (string.IsNullOrWhiteSpace(req.ProjectId))
            throw new ClientValidationException(MessageCode.ProjectIdIsRequired);
        if (!Guid.TryParse(req.ProjectId, out var projectId))
            throw new ClientValidationException("PROJECT_ID_INVALID_FORMAT");
        
        var dto = mapper.Map<CreateDatasetDto>(req);
        
        dto.ProjectId = projectId;
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
        
        var command = new CreateDatasetCommand(dto);
        var result = await sender.Send(command);
        
        return new ApiCreatedResponse<Guid>(result);
    }
    
    #endregion
}
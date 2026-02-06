using AutoMapper;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Swagger.Extensions;
using Common.Constants;
using Common.Models;
using Management.Api.Constants;
using Management.Api.Models;
using Management.Application.Dtos.Datasets;
using Management.Application.Features.Dataset.Commands;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class UpdateDataset : ICarterModule
{
    #region Implementations
    
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Dataset.Update, HandleUpdateDatasetAsync)
            .WithTags(ApiRoutes.Dataset.Tags)
            .WithName(nameof(UpdateDataset))
            .WithMultipartForm<UpdateDatasetDto>()
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }
    
    #endregion

    #region Methods
    
    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateDatasetAsync(
        ISender sender,
        IMapper mapper,
        [FromRoute] Guid datasetId,
        [FromForm] UpdateDatasetRequest req
    )
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);
        
        var dto = mapper.Map<UpdateDatasetDto>(req);
    
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
        
        var command = new UpdateDatasetCommand(datasetId,dto);
        var result = await sender.Send(command);
        
        return new ApiUpdatedResponse<Guid>(result);
    }
    
    #endregion
}
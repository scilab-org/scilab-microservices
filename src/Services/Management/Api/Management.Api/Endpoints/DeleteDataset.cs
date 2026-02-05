using Management.Api.Constants;
using Management.Application.Features.Dataset.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteDataset : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Dataset.Delete, HandleDeleteDatasetAsync)
            .WithTags(ApiRoutes.Dataset.Tags)
            .WithName(nameof(DeleteDataset))
            .Produces<ApiDeletedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiDeletedResponse<Guid>> HandleDeleteDatasetAsync(
        ISender sender,
        [FromRoute] Guid datasetId)
    {
        var command = new DeleteDatasetCommand(datasetId);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(datasetId);
    }
    #endregion
}
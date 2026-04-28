using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperBank.Commands.RetryPaperBankIngestion;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class RetryPaperBankIngestion : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.PaperBank.RetryIngestion, HandleRetryIngestionAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(RetryPaperBankIngestion))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<Guid>> HandleRetryIngestionAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new RetryPaperBankIngestionCommand(id);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }

    #endregion
}

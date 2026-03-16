using Lab.Api.Constants;
using Lab.Application.Features.PaperBank.Commands.DeletePaperBank;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeletePaperBank : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.PaperBank.Delete, HandleDeleteBankAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(DeletePaperBank))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteBankAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeletePaperBankCommand(id);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
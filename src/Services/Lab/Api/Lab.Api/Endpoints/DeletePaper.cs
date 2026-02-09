using Lab.Api.Constants;
using Lab.Application.Features.Paper.Commands.DeletePaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeletePaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Paper.Delete, HandleDeletePaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(DeletePaper))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeletePaperAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeletePaperCommand(id);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
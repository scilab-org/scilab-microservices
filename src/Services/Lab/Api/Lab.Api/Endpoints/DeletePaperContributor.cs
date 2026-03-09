using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Commands.DeletePaperContributor;
using Lab.Application.Features.Template.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeletePaperContributor: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.PaperContributor.Delete, HandleDeletePaperContributor)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(DeletePaperContributor))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeletePaperContributor(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeletePaperContributorCommand(id);
        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
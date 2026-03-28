using Management.Api.Constants;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class DeleteProjectPaperByBankId: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ProjectPaper.DeleteProjectPaperByBankId, HandleDeleteProjectPapersAsync)
            .WithTags(ApiRoutes.ProjectPaper.Tags)
            .WithName(nameof(DeleteProjectPaperByBankId))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteProjectPapersAsync(
        ISender sender,
        [FromRoute] Guid paperBankId)
    {
        var command = new DeleteProjectPaperByBankIdCommand(paperBankId);

        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }

    #endregion
}

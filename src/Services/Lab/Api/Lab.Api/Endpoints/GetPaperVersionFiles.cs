using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFiles;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetPaperVersionFiles : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetVersionFiles, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperVersionFiles))
            .Produces<GetPaperVersionFilesResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid paperId,
        [FromRoute] Guid versionId)
    {
        var query = new GetPaperVersionFilesQuery(paperId, versionId);
        var result = await sender.Send(query);

        return Results.Ok(result);
    }

    #endregion
}

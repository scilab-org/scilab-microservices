using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Queries.GetPaperVersionFileById;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetPaperVersionFileById : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetVersionFileById, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperVersionFileById))
            .Produces<PaperVersionFileDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetPaperVersionFileByIdQuery(id);
        var result = await sender.Send(query);

        return Results.Ok(result);
    }

    #endregion
}

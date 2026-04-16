using Lab.Api.Constants;
using Lab.Application.Features.Paper.Queries.GetPaperStatusHistory;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetPaperStatusHistory : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetStatusHistory, HandleAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperStatusHistory))
            .Produces<GetPaperStatusHistoryResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAsync(
        ISender sender,
        Guid id)
    {
        var query = new GetPaperStatusHistoryQuery(id);
        var result = await sender.Send(query);

        return Results.Ok(result);
    }

    #endregion
}

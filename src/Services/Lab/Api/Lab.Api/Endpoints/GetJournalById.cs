using Lab.Api.Constants;
using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Models.Results;
using Common.Models.Reponses;

namespace Lab.Api.Endpoints;

public class GetJournalById : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Journal.GetJournalById, HandleGetJournalByIdAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(GetJournalById))
            .Produces<ApiGetResponse<GetJournalByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetJournalByIdResult>> HandleGetJournalByIdAsync(
        ISender sender,
        Guid id)
    {
        var query = new GetJournalByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetJournalByIdResult>(result);
    }

    #endregion
}
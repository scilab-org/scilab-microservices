using Lab.Api.Constants;
using Lab.Application.Features.Journal.Queries.GetJournalById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetJournalInProjectById : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Journal.GetJournalInProjectById, HandleGetJournalByIdAsync)
            .WithTags(ApiRoutes.Journal.Tags)
            .WithName(nameof(GetJournalInProjectById))
            .Produces<ApiGetResponse<GetJournalByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetJournalByIdResult>> HandleGetJournalByIdAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromRoute] Guid projectId)
    {
        var query = new GetJournalInProjectByIdQuery(id, projectId);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetJournalByIdResult>(result);
    }

    #endregion
}
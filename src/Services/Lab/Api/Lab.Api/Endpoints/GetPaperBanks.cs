using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperBank.Queries.GetPaperBanks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;

namespace Lab.Api.Endpoints;

public class GetPaperBanks : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperBank.GetPaperBanks, HandleGetPaperBanksAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(GetPaperBanks))
            .Produces<ApiGetResponse<GetPaperBanksResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<ApiGetResponse<GetPaperBanksResult>> HandleGetPaperBanksAsync(
        ISender sender,
        [AsParameters] GetPaperBanksFilter req,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetPaperBanksQuery(req, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPaperBanksResult>(result);
    }

    #endregion
}
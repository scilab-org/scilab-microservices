using Lab.Application.Features.Paper.Queries.GetPaperById;

namespace Lab.Api.Endpoints;
using Lab.Api.Constants;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

public class GetPaperById: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetPaperById, HandleGetPaperBankByIdAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetPaperById))
            .Produces<ApiGetResponse<GetPaperByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    private async Task<ApiGetResponse<GetPaperByIdResult>> HandleGetPaperBankByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetPaperByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPaperByIdResult>(result);
    }
    
}
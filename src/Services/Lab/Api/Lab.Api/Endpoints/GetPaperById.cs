using Lab.Api.Constants;
using Lab.Application.Features.PaperBank.Queries.GetPaperBankById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetPaperBankById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperBank.GetPaperBankById, HandleGetPaperBankByIdAsync)
            .WithTags(ApiRoutes.PaperBank.Tags)
            .WithName(nameof(GetPaperBankById))
            .Produces<ApiGetResponse<GetPaperBankByIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    private async Task<ApiGetResponse<GetPaperBankByIdResult>> HandleGetPaperBankByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetPaperBankByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetPaperBankByIdResult>(result);
    }
}
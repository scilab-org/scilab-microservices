using Lab.Api.Constants;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthorById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetPaperAuthorById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperAuthor.GetPaperAuthorById, HandleGetPaperAuthorByIdAsync)
            .WithTags(ApiRoutes.PaperAuthor.Tags)
            .WithName(nameof(GetPaperAuthorById))
            .Produces<ApiGetResponse<GetPaperAuthorByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetPaperAuthorByIdResult>> HandleGetPaperAuthorByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetPaperAuthorByIdQuery(id));
        return new ApiGetResponse<GetPaperAuthorByIdResult>(result);
    }
}

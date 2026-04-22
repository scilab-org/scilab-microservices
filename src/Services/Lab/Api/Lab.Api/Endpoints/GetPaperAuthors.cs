using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperAuthor.Queries.GetPaperAuthors;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetPaperAuthors : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PaperAuthor.GetPaperAuthors, HandleGetPaperAuthorsAsync)
            .WithTags(ApiRoutes.PaperAuthor.Tags)
            .WithName(nameof(GetPaperAuthors))
            .Produces<ApiGetResponse<GetPaperAuthorsResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetPaperAuthorsResult>> HandleGetPaperAuthorsAsync(
        ISender sender,
        [AsParameters] GetPaperAuthorsFilter filter,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetPaperAuthorsQuery(filter, paging);
        var result = await sender.Send(query);
        return new ApiGetResponse<GetPaperAuthorsResult>(result);
    }
}

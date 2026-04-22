using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Domain.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetDomains : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Domain.GetDomains, HandleGetDomainsAsync)
            .WithTags(ApiRoutes.Domain.Tags)
            .WithName(nameof(GetDomains))
            .Produces<ApiGetResponse<GetDomainsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetDomainsResult>> HandleGetDomainsAsync(
        ISender sender,
        [FromQuery] string? name,
        [AsParameters] PaginationRequest paging)
    {
        var result = await sender.Send(new GetDomainsQuery(paging, name));
        return new ApiGetResponse<GetDomainsResult>(result);
    }
}

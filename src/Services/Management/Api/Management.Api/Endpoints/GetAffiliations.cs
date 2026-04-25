using Common.Models;
// cspell:disable-next-line
using Common.Models.Reponses;
using Management.Api.Constants;
using Management.Application.Features.Affiliation.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAffiliations : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Affiliation.GetAffiliations, HandleGetAffiliationsAsync)
            .WithTags(ApiRoutes.Affiliation.Tags)
            .WithName(nameof(GetAffiliations))
            .Produces<ApiGetResponse<GetAffiliationsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetAffiliationsResult>> HandleGetAffiliationsAsync(
        ISender sender,
        [FromQuery] string? name,
        [AsParameters] PaginationRequest paging)
    {
        var result = await sender.Send(new GetAffiliationsQuery(paging, name));
        return new ApiGetResponse<GetAffiliationsResult>(result);
    }
}

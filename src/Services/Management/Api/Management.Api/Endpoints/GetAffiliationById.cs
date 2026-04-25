using Management.Api.Constants;
using Management.Application.Dtos.Affiliations;
using Management.Application.Features.Affiliation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetAffiliationById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Affiliation.GetAffiliationById, HandleGetAffiliationByIdAsync)
            .WithTags(ApiRoutes.Affiliation.Tags)
            .WithName(nameof(GetAffiliationById))
            .Produces<ApiGetResponse<AffiliationDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<AffiliationDto>> HandleGetAffiliationByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetAffiliationByIdQuery(id));
        return new ApiGetResponse<AffiliationDto>(result);
    }
}

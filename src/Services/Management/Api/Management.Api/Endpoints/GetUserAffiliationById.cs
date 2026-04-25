using Management.Api.Constants;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Features.UserAffiliation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetUserAffiliationById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.UserAffiliation.GetUserAffiliationById, HandleGetUserAffiliationByIdAsync)
            .WithTags(ApiRoutes.UserAffiliation.Tags)
            .WithName(nameof(GetUserAffiliationById))
            .Produces<ApiGetResponse<UserAffiliationDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<UserAffiliationDto>> HandleGetUserAffiliationByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetUserAffiliationByIdQuery(id));
        return new ApiGetResponse<UserAffiliationDto>(result);
    }
}

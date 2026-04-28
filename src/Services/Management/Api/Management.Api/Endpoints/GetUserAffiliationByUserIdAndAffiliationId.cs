using Management.Api.Constants;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Features.UserAffiliation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetUserAffiliationByUserIdAndAffiliationId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.UserAffiliation.GetUserAffiliationByUserIdAndAffiliationId, HandleGetUserAffiliationByUserIdAndAffiliationIdAsync)
            .WithTags(ApiRoutes.UserAffiliation.Tags)
            .WithName(nameof(GetUserAffiliationByUserIdAndAffiliationId))
            .Produces<ApiGetResponse<UserAffiliationDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<UserAffiliationDto>> HandleGetUserAffiliationByUserIdAndAffiliationIdAsync(
        ISender sender,
        [FromRoute] Guid userId,
        [FromRoute] Guid affiliationId)
    {
        var result = await sender.Send(new GetUserAffiliationByUserIdAndAffiliationIdQuery(userId, affiliationId));
        return new ApiGetResponse<UserAffiliationDto>(result);
    }
}

using Management.Api.Constants;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Features.UserAffiliation.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetUserAffiliations : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.UserAffiliation.GetUserAffiliations, HandleGetUserAffiliationsAsync)
            .WithTags(ApiRoutes.UserAffiliation.Tags)
            .WithName(nameof(GetUserAffiliations))
            .Produces<ApiGetResponse<List<UserAffiliationDto>>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<List<UserAffiliationDto>>> HandleGetUserAffiliationsAsync(ISender sender)
    {
        var result = await sender.Send(new GetUserAffiliationsQuery());
        return new ApiGetResponse<List<UserAffiliationDto>>(result);
    }
}
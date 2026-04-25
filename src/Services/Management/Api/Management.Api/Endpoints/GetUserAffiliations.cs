using BuildingBlocks.Exceptions;
using Common.Constants;
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

    private async Task<ApiGetResponse<List<UserAffiliationDto>>> HandleGetUserAffiliationsAsync(
        ISender sender,
        [FromQuery] string userId)
    {
        var id = Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
        if (id == Guid.Empty)
            throw new ClientValidationException(MessageCode.UserIdIsRequired, userId);
        var result = await sender.Send(new GetUserAffiliationsQuery(id));
        return new ApiGetResponse<List<UserAffiliationDto>>(result);
    }
}
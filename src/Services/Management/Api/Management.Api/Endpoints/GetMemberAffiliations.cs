using Common.Models;
// cspell:disable-next-line
using Common.Models.Reponses;
using Management.Api.Constants;
using Management.Application.Features.UserAffiliation.Queries;
using Management.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetMemberAffiliations : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Member.GetMemberAffiliations, HandleGetMemberAffiliationsAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(GetMemberAffiliations))
            .Produces<ApiGetResponse<GetMemberAffiliationsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetMemberAffiliationsResult>> HandleGetMemberAffiliationsAsync(
        ISender sender,
        [FromRoute] Guid memberId,
        [AsParameters] PaginationRequest paging,
        [FromQuery] string? affiliationName)
    {
        var result = await sender.Send(new GetMemberAffiliationsQuery(memberId, paging, affiliationName));
        return new ApiGetResponse<GetMemberAffiliationsResult>(result);
    }
}

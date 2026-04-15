using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Queries.GetMemberById;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetMemberById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Member.GetMemberById, HandleAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(GetMemberById))
            .Produces<ApiGetResponse<MemberDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleAsync(
        ISender sender,
        [FromRoute] Guid memberId)
    {
        var result = await sender.Send(new GetMemberByIdQuery(memberId));
        if (result == null)
            return Results.NotFound();

        return TypedResults.Ok(new ApiGetResponse<MemberDto>(result));
    }
}

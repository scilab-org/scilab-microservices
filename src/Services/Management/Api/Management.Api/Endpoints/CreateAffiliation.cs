using Management.Api.Constants;
using Management.Application.Dtos.Affiliations;
using Management.Application.Features.Affiliation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class CreateAffiliation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Affiliation.Create, HandleCreateAffiliationAsync)
            .WithTags(ApiRoutes.Affiliation.Tags)
            .WithName(nameof(CreateAffiliation))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> HandleCreateAffiliationAsync(ISender sender, [FromBody] CreateAffiliationDto req)
    {
        var result = await sender.Send(new CreateAffiliationCommand(req));
        return TypedResults.Created($"{ApiRoutes.Affiliation.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

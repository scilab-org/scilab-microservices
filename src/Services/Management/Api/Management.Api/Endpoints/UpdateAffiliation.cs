using Management.Api.Constants;
using Management.Application.Dtos.Affiliations;
using Management.Application.Features.Affiliation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class UpdateAffiliation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Affiliation.Update, HandleUpdateAffiliationAsync)
            .WithTags(ApiRoutes.Affiliation.Tags)
            .WithName(nameof(UpdateAffiliation))
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateAffiliationAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateAffiliationDto req)
    {
        var result = await sender.Send(new UpdateAffiliationCommand(id, req));
        return new ApiUpdatedResponse<Guid>(result);
    }
}

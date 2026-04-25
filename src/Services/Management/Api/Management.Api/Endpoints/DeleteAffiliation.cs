using Management.Api.Constants;
using Management.Application.Features.Affiliation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteAffiliation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Affiliation.Delete, HandleDeleteAffiliationAsync)
            .WithTags(ApiRoutes.Affiliation.Tags)
            .WithName(nameof(DeleteAffiliation))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteAffiliationAsync(ISender sender, [FromRoute] Guid id)
    {
        await sender.Send(new DeleteAffiliationCommand(id));
        return new ApiDeletedResponse<Guid>(id);
    }
}

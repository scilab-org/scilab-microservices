using Management.Api.Constants;
using Management.Application.Features.UserAffiliation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteUserAffiliation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.UserAffiliation.Delete, HandleDeleteUserAffiliationAsync)
            .WithTags(ApiRoutes.UserAffiliation.Tags)
            .WithName(nameof(DeleteUserAffiliation))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteUserAffiliationAsync(
        ISender sender, 
        [FromRoute] Guid id)
    {
        await sender.Send(new DeleteUserAffiliationCommand(id));
        return new ApiDeletedResponse<Guid>(id);
    }
}

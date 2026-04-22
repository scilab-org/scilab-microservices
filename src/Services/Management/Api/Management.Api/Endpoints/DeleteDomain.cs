using Management.Api.Constants;
using Management.Application.Features.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteDomain : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Domain.Delete, HandleDeleteDomainAsync)
            .WithTags(ApiRoutes.Domain.Tags)
            .WithName(nameof(DeleteDomain))
            .Produces<ApiDeletedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteDomainAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        await sender.Send(new DeleteDomainCommand(id));
        return new ApiDeletedResponse<Guid>(id);
    }
}

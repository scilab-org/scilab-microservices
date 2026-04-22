using Management.Api.Constants;
using Management.Application.Dtos.Domains;
using Management.Application.Features.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class UpdateDomain : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.Domain.Update, HandleUpdateDomainAsync)
            .WithTags(ApiRoutes.Domain.Tags)
            .WithName(nameof(UpdateDomain))
            .Produces<ApiUpdatedResponse<Guid>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdateDomainAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateDomainDto req)
    {
        var command = new UpdateDomainCommand(id, req);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }
}

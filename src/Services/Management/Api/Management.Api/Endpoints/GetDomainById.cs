using Management.Api.Constants;
using Management.Application.Dtos.Domains;
using Management.Application.Features.Domain.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class GetDomainById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Domain.GetDomainById, HandleGetDomainByIdAsync)
            .WithTags(ApiRoutes.Domain.Tags)
            .WithName(nameof(GetDomainById))
            .Produces<ApiGetResponse<DomainDto>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<DomainDto>> HandleGetDomainByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetDomainByIdQuery(id));
        return new ApiGetResponse<DomainDto>(result);
    }
}

using Management.Api.Constants;
using Management.Application.Dtos.Domains;
using Management.Application.Features.Domain.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class CreateDomain : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Domain.Create, HandleCreateDomainAsync)
            .WithTags(ApiRoutes.Domain.Tags)
            .WithName(nameof(CreateDomain))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<IResult> HandleCreateDomainAsync(ISender sender, [FromBody] CreateDomainDto req)
    {
        var command = new CreateDomainCommand(req);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Domain.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

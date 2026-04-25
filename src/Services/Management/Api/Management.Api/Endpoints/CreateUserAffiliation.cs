using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.UserAffiliations;
using Management.Application.Features.UserAffiliation.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class CreateUserAffiliation : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.UserAffiliation.Create, HandleCreateUserAffiliationAsync)
            .WithTags(ApiRoutes.UserAffiliation.Tags)
            .WithName(nameof(CreateUserAffiliation))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleCreateUserAffiliationAsync(
        ISender sender, 
        IHttpContextAccessor httpContext,
        [FromBody] CreateUserAffiliationDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            return Results.Unauthorized();
        
        var result = await sender.Send(new CreateUserAffiliationCommand(req));
        return TypedResults.Created($"{ApiRoutes.UserAffiliation.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;
using Management.Api.Constants;

namespace Management.Api.Endpoints;

public sealed class AddProjectMembers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Member.AddProjectMembers, HandleAddProjectMembersAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(AddProjectMembers))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleAddProjectMembersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId,
        [FromBody] AddProjectMembersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new AddProjectMembersCommand(projectId, req, userId);

        var result = await sender.Send(command);

        return TypedResults.Created(
            $"/projects/{projectId}/members",
            new ApiCreatedResponse<List<Guid>>(result));
    }

    #endregion
}

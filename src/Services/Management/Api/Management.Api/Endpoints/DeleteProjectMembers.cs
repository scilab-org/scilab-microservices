using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteProjectMembers : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Member.DeleteProjectMembers, HandleDeleteProjectMembersAsync)
            .WithTags(ApiRoutes.Member.Tags)
            .WithName(nameof(DeleteProjectMembers))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteProjectMembersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid projectId,
        [FromBody] DeleteProjectMembersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new DeleteProjectMembersCommand(projectId, req, currentUser.Id);

        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }

    #endregion
}


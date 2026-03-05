using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class DeleteSubProjectMember : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.SubProject.DeleteSubProjectMembers, HandleDeleteSubProjectMembersAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(DeleteSubProjectMember))
            .Produces<ApiDeletedResponse<List<Guid>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleDeleteSubProjectMembersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid subProjectId,
        [FromBody] DeleteProjectMembersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new DeleteSubProjectMembersCommand(subProjectId, req, userId);

        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiDeletedResponse<List<Guid>>(result));
    }

    #endregion
}


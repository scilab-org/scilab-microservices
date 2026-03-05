using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Features.Member.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public class AddSubProjectMembers: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.SubProject.AddSubProjectMember, AddSubProjectMembersAsync)
            .WithTags(ApiRoutes.SubProject.Tags)
            .WithName(nameof(AddSubProjectMembers))
            .Produces<ApiCreatedResponse<List<Guid>>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> AddSubProjectMembersAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid subProjectId,
        [FromBody] AddProjectMembersDto req)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();
        
        var command = new AddSubProjectMembersCommand(subProjectId, req, userId);

        var result = await sender.Send(command);

        return TypedResults.Created(
            $"/projects/{subProjectId}/members",
            new ApiCreatedResponse<List<Guid>>(result));
    }

    #endregion
}
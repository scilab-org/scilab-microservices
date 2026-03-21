using BuildingBlocks.Authentication.Extensions;
using Lab.Api.Constants;
using Lab.Application.Dtos.Comments;
using Lab.Application.Features.Comment.Commands.CreateComment;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreateComment: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Comment.Create, HandleCreateCommentAsync)
            .WithTags(ApiRoutes.Comment.Tags)
            .WithName(nameof(CreateComment))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreateCommentAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreateCommentDto dto)
    {
        var currentUser = httpContext.GetCurrentUser();
        if (string.IsNullOrWhiteSpace(currentUser.Id))
            return Results.Unauthorized();
        
        var command = new CreateCommentCommand(currentUser.UserName, dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Paper.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
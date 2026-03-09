using BuildingBlocks.Authentication.Extensions;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSections;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetAssignedPaperSections: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetAssignedPaperSections, HandleGetMySectionsAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(GetAssignedPaperSections))
            .Produces<ApiGetResponse<GetMySectionsResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    #endregion

    #region Methods
    private async Task<IResult> HandleGetMySectionsAsync(
        ISender sender,
        [FromRoute] Guid id,
        [AsParameters] PaginationRequest paging,
        IHttpContextAccessor httpContext)
    {
        var currentUser = httpContext.GetCurrentUser();

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var query = new GetAssignedPaperSectionsQuery(id, userId, paging);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetMySectionsResult>(result));
    }

    #endregion
}
using BuildingBlocks.Authentication.Extensions;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.PaperContributor.Queries.GetAssignedPaperSectionsHistory;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetAssignedPaperSectionsHistory : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Paper.GetAssignedPaperSectionsHistory, HandleGetMySectionsHistoryAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(GetAssignedPaperSectionsHistory))
            .Produces<ApiGetResponse<GetAssignedPaperSectionsHistoryResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    private async Task<IResult> HandleGetMySectionsHistoryAsync(
        ISender sender,
        [FromRoute] Guid id,
        [AsParameters] PaginationRequest paging,
        [AsParameters] GetAssignedPaperSectionsHistoryFilter filter,
        IHttpContextAccessor httpContext)
    {
        var currentUser = httpContext.GetCurrentUser();

        if (string.IsNullOrWhiteSpace(currentUser.Id) || !Guid.TryParse(currentUser.Id, out var userId))
            return Results.Unauthorized();

        var query = new GetAssignedPaperSectionsHistoryQuery(id, userId, filter, paging);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetAssignedPaperSectionsHistoryResult>(result));
    }
}

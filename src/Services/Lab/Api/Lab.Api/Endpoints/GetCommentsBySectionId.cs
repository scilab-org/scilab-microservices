using Lab.Api.Constants;
using Lab.Application.Features.Comment.Queries.GetCommentsBySectionId;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetCommentsBySectionId: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Comment.GetCommentsBySectionId, HandleGetCommentsBySectionIdAsync)
            .WithTags(ApiRoutes.Comment.Tags)
            .WithName(nameof(GetCommentsBySectionId))
            .Produces<ApiGetResponse<GetCommentsBySectionIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
    
    private async Task<ApiGetResponse<GetCommentsBySectionIdResult>> HandleGetCommentsBySectionIdAsync(
        ISender sender,
        [FromRoute] Guid sectionId)
    {
        var query = new GetCommentsBySectionIdQuery(sectionId);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetCommentsBySectionIdResult>(result);
    }
}
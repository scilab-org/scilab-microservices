using Lab.Api.Constants;
using Lab.Application.Dtos.Sections;
using Lab.Application.Features.Section.Queries.GetPreviewReference;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetPreviewReference : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Section.PreviewReference, HandleGetPreviewReferenceAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetPreviewReference))
            .Produces<ApiGetResponse<GetInUseReferenceBySectionIdResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private static async Task<IResult> HandleGetPreviewReferenceAsync(
        ISender sender,
        [FromBody] PreviewReferenceDto dto)
    {
        var query = new GetPreviewReferenceQuery(dto);
        var result = await sender.Send(query);

        return TypedResults.Ok(new ApiGetResponse<GetInUseReferenceBySectionIdResult>(result));
    }
}
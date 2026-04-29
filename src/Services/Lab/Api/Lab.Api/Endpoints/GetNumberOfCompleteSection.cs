using Lab.Api.Constants;
using Lab.Application.Features.Section.Queries.GetNumberOfCompleteSection;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetNumberOfCompleteSection : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Section.GetNumberOfCompleteSection, GetNumberOfCompleteSectionHandler)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(GetNumberOfCompleteSection))
            .Produces<ApiGetResponse<GetNumberOfCompleteSection>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetNumberOfCompleteSectionResult>> GetNumberOfCompleteSectionHandler(
        ISender sender,
        [FromRoute] Guid id)
    {
        var result = await sender.Send(new GetNumberOfCompleteSectionQuery(id));
        return new ApiGetResponse<GetNumberOfCompleteSectionResult>(result);
    }
    
}
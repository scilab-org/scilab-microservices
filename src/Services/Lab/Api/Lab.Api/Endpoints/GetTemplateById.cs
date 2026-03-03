using Lab.Api.Constants;
using Lab.Application.Features.Template.Queries.GetTemplateById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetTemplateById : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Template.GetTemplateById, HandleGetTemplateByIdAsync)
            .WithTags(ApiRoutes.Template.Tags)
            .WithName(nameof(GetTemplateById))
            .Produces<ApiGetResponse<GetTemplateByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetTemplateByIdResult>> HandleGetTemplateByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetTemplateByIdQuery(id);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetTemplateByIdResult>(result);
    }

    #endregion
}
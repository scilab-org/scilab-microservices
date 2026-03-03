using Lab.Api.Constants;
using Lab.Application.Dtos.Template;
using Lab.Application.Features.Template.Queries.GetTemplatesByCode;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class GetTemplatesByCode : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Template.GetByCode, HandleGetTemplatesByCodeAsync)
            .WithTags(ApiRoutes.Template.Tags)
            .WithName(nameof(GetTemplatesByCode))
            .Produces<ApiGetResponse<GetTemplatesByCodeResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<TemplateDto>> HandleGetTemplatesByCodeAsync(
        ISender sender,
        [FromRoute] string code)
    {
        var query = new GetTemplatesByCodeQuery(code);
        var result = await sender.Send(query);

        return new ApiGetResponse<TemplateDto>(result);
    }

    #endregion
}
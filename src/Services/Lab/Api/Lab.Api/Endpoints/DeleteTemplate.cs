using Lab.Api.Constants;
using Lab.Application.Features.Template.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeleteTemplate : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Template.Delete, HandleDeleteTemplate)
            .WithTags(ApiRoutes.Template.Tags)
            .WithName(nameof(DeleteTemplate))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteTemplate(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeleteTemplateCommand(id);
        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
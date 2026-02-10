using Lab.Api.Constants;
using Lab.Application.Features.Tag.Commands.DeleteTag;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeleteTag : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Tag.Delete, HandleDeleteTagAsync)
            .WithTags(ApiRoutes.Tag.Tags)
            .WithName(nameof(DeleteTag))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteTagAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var command = new DeleteTagCommand(id);

        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }

    #endregion
}
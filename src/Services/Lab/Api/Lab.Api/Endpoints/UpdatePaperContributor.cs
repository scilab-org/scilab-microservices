using Lab.Api.Constants;
using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;
using Lab.Application.Features.PaperContributor.Commands.UpdatePaperContributor;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdatePaperContributor: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.PaperContributor.Update, HandleUpdatePaperContributorAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(UpdatePaperContributor))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiUpdatedResponse<Guid>> HandleUpdatePaperContributorAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdatePaperContributorDto req)
    {
        var command = new UpdatePaperContributorCommand(id, req);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
        
    }

    #endregion
}
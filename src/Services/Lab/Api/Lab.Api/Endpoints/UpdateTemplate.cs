using Lab.Api.Constants;
using Lab.Application.Dtos.Template;
using Lab.Application.Features.Template.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class UpdateTemplate : ICarterModule
{
    #region Implementation

     public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut(ApiRoutes.Template.Update, HandleCreateTemplateVersion)
                .WithTags(ApiRoutes.Template.Tags)
                .WithName(nameof(UpdateTemplate))
                .Produces<ApiUpdatedResponse<Guid>>()
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

    #endregion

    #region Methods
    
    private async Task<ApiUpdatedResponse<Guid>> HandleCreateTemplateVersion(
        ISender sender,
        [FromRoute] Guid id,
        CreateTemplateVersionDto req)
    {
        var command = new UpdateTemplateCommand(id, req);
        var result = await sender.Send(command);

        return new ApiUpdatedResponse<Guid>(result);
    }

    #endregion
}
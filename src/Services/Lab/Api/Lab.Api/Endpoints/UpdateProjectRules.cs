using Lab.Api.Constants;
using Lab.Application.Dtos.Projects;
using Lab.Application.Features.System;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdateProjectRules : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.System.UpdateProjectRules, HandleAsync)
            .WithTags(ApiRoutes.System.Tags)
            .WithName(nameof(UpdateProjectRules))
            .Produces<ApiUpdatedResponse<bool>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }

    private async Task<ApiUpdatedResponse<bool>> HandleAsync(
        ISender sender,
        [FromBody] UpdateProjectRulesDto request)
    {
        var command = new UpdateProjectRulesCommand(request);
        var result = await sender.Send(command);
        return new ApiUpdatedResponse<bool>(result);
    }
}
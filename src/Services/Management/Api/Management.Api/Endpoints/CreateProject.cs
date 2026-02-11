using BuildingBlocks.Authentication.Extensions;
using Management.Api.Constants;
using Management.Application.Dtos.Projects;
using Management.Application.Features.Project.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Management.Api.Endpoints;

public sealed class CreateProject : ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Project.Create, HandleCreateProjectAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(CreateProject))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest) ;
        // .RequireAuthorization();
    }
    
    #endregion

    #region Methods

    private async Task<IResult> HandleCreateProjectAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreateProjectDto req)
    {
        var command = new CreateProjectCommand(req);

        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Project.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
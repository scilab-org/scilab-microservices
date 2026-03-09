using AutoMapper;
using Lab.Api.Constants;
using Lab.Application.Dtos.PaperContributors;
using Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class CreatePaperContributor: ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.PaperContributor.Create, HandleCreatePaperContributorAsync)
            .WithTags(ApiRoutes.PaperContributor.Tags)
            .WithName(nameof(CreatePaperContributor))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleCreatePaperContributorAsync(
        ISender sender,
        [FromBody] CreatePaperContributorDto req)
    {
        var command = new CreatePaperContributorCommand(req);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.PaperContributor.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
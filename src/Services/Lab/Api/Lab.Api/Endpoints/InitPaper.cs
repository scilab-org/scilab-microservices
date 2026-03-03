using Lab.Api.Constants;
using Lab.Application.Dtos.Papers;
using Lab.Application.Features.Paper.Commands.InitPaper;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class InitPaper : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Paper.Initialize, HandleInitPaperAsync)
            .WithTags(ApiRoutes.Paper.Tags)
            .WithName(nameof(InitPaper))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery();
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<IResult> HandleInitPaperAsync(
        ISender sender,
        [FromBody] InitPaperDto dto)
    {
        var command = new InitPaperCommand(dto);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Paper.Initialize}/{result}", new ApiCreatedResponse<Guid>(result));
    }

    #endregion
}
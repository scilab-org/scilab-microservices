using BuildingBlocks.Exceptions;
using Common.Constants;
using Lab.Api.Constants;
using Lab.Application.Dtos.Template;
using Lab.Application.Features.Template.Commands;

namespace Lab.Api.Endpoints;

public sealed class CreateTemplate : ICarterModule
{
    #region Implementations
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Template.Create, HandleCreatePaperTemplate)
            .WithTags(ApiRoutes.Template.Tags)
            .WithName(nameof(CreateTemplate))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        // .RequireAuthorization();
    }
    
    #endregion
    
    #region Methods
    
    private async Task<IResult> HandleCreatePaperTemplate(
        ISender sender,
        CreateTemplateDto req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var command = new CreateTemplateCommand(req);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.Template.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
    #endregion
}
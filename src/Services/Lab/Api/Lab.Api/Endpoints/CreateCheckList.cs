using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.CheckList;
using Lab.Application.Dtos.CheckLists;
using Lab.Application.Features.CheckList.Commands.CreateCheckList;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class CreateCheckList : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.CheckList.Create, HandleCreateCheckListAsync)
            .WithTags(ApiRoutes.CheckList.Tags)
            .WithName(nameof(CreateCheckList))
            .Produces<ApiCreatedResponse<Guid>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<IResult> HandleCreateCheckListAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromBody] CreateCheckListRequest req)
    {
        if (req == null) throw new ClientValidationException(MessageCode.BadRequest);

        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var dto = new CreateCheckListDto
        {
            Section = req.Section,
            Items = req.Items.Select(x => new CheckListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Rule = x.Rule,
                Weight = x.Weight
            }).ToList()
        };

        var command = new CreateCheckListCommand(dto, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Created($"{ApiRoutes.CheckList.Create}/{result}", new ApiCreatedResponse<Guid>(result));
    }
}

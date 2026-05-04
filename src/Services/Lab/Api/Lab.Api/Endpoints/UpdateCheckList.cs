using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Exceptions;
using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Api.Models.CheckList;
using Lab.Application.Dtos.CheckLists;
using Lab.Application.Features.CheckList.Commands.UpdateCheckList;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class UpdateCheckList : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.CheckList.Update, HandleUpdateCheckListAsync)
            .WithTags(ApiRoutes.CheckList.Tags)
            .WithName(nameof(UpdateCheckList))
            .Produces<ApiUpdatedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    private async Task<IResult> HandleUpdateCheckListAsync(
        ISender sender,
        IHttpContextAccessor httpContext,
        [FromRoute] Guid id,
        [FromBody] UpdateCheckListRequest request)
    {
        if (request == null) throw new ClientValidationException(MessageCode.BadRequest);

        var currentUser = httpContext.GetCurrentUser();
        if (currentUser == null)
            throw new UnauthorizedException(MessageCode.Unauthorized);

        var dto = new UpdateCheckListDto
        {
            Section = request.Section,
            Items = request.Items.Select(x => new CheckListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Rule = x.Rule,
                Weight = x.Weight
            }).ToList()
        };

        var command = new UpdateCheckListCommand(dto, id, currentUser.UserName);
        var result = await sender.Send(command);

        return TypedResults.Ok(new ApiUpdatedResponse<Guid>(result));
    }
}

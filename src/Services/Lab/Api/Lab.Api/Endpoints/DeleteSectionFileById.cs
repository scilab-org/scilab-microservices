using Common.Constants;
using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.Section.Commands.DeleteSectionFile;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public class DeleteSectionFileById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.Section.DeleteSectionFileById, HandleDeleteSectionFileByIdAsync)
            .WithTags(ApiRoutes.Section.Tags)
            .WithName(nameof(DeleteSectionFileById))
            .Produces<ApiDeletedResponse<Guid>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private async Task<ApiDeletedResponse<Guid>> HandleDeleteSectionFileByIdAsync(
        ISender sender,
        [FromRoute] Guid id,
        [FromRoute] string fileName)
    {
        var command = new DeleteSectionFileCommand(id, fileName);
        await sender.Send(command);

        return new ApiDeletedResponse<Guid>(id);
    }
}

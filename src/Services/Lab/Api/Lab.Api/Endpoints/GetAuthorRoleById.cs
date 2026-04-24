using Lab.Api.Constants;
using Lab.Application.Features.AuthorRole.Queries.GetAuthorRoleById;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetAuthorRoleById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.AuthorRole.GetAuthorRoleById, HandleGetAuthorRoleByIdAsync)
            .WithTags(ApiRoutes.AuthorRole.Tags)
            .WithName(nameof(GetAuthorRoleById))
            .Produces<ApiGetResponse<GetAuthorRoleByIdResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetAuthorRoleByIdResult>> HandleGetAuthorRoleByIdAsync(
        ISender sender,
        [FromRoute] Guid id)
    {
        var query = new GetAuthorRoleByIdQuery(id);
        var result = await sender.Send(query);
        return new ApiGetResponse<GetAuthorRoleByIdResult>(result);
    }
}

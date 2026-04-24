using Common.Models;
using Lab.Api.Constants;
using Lab.Application.Features.AuthorRole.Queries.GetAuthorRoles;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Endpoints;

public sealed class GetAuthorRoles : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.AuthorRole.GetAuthorRoles, HandleGetAuthorRolesAsync)
            .WithTags(ApiRoutes.AuthorRole.Tags)
            .WithName(nameof(GetAuthorRoles))
            .Produces<ApiGetResponse<GetAuthorRolesResult>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private async Task<ApiGetResponse<GetAuthorRolesResult>> HandleGetAuthorRolesAsync(
        ISender sender,
        [FromQuery] string? name,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetAuthorRolesQuery(name, paging);
        var result = await sender.Send(query);
        return new ApiGetResponse<GetAuthorRolesResult>(result);
    }
}

using Common.Models;
using Management.Api.Constants;
using Management.Application.Features.Project.Queries;
using Management.Application.Models.Results;

namespace Management.Api.Endpoints;

public sealed class GetProjectsByUserId : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Project.GetProjectsByUserId, HandleGetProjectsByUserIdAsync)
            .WithTags(ApiRoutes.Project.Tags)
            .WithName(nameof(GetProjectsByUserId))
            .Produces<ApiGetResponse<GetProjectsResult>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<GetProjectsResult>> HandleGetProjectsByUserIdAsync(
        ISender sender,
        Guid userId,
        [AsParameters] PaginationRequest paging)
    {
        var query = new GetProjectsByUserIdQuery(userId, paging);
        var result = await sender.Send(query);

        return new ApiGetResponse<GetProjectsResult>(result);
    }

    #endregion
}


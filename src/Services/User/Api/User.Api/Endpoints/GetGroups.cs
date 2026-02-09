#region using

using User.Api.Constants;
using User.Application.Dtos.Groups;
using User.Application.Features.Groups.Queries;

#endregion

namespace User.Api.Endpoints;

public sealed class GetGroups : ICarterModule
{
    #region Implementations

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Groups.GetAll, HandleGetGroupsAsync)
            .WithTags(ApiRoutes.Groups.Tags)
            .WithName(nameof(GetGroups))
            .Produces<ApiGetResponse<List<GroupDto>>>(StatusCodes.Status200OK);
        // .RequireAuthorization();
    }

    #endregion

    #region Methods

    private async Task<ApiGetResponse<List<GroupDto>>> HandleGetGroupsAsync(
        ISender sender)
    {
        var query = new GetGroupsQuery();

        var result = await sender.Send(query);

        return new ApiGetResponse<List<GroupDto>>(result);
    }

    #endregion
}

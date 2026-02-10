#region using

using User.Application.Dtos.Users;

#endregion

namespace User.Application.Models.Results;

public sealed class GetUsersResult
{
    #region Fields, Properties and Indexers

    public List<UserDto> Items { get; init; }

    public PagingResult Paging { get; init; }

    #endregion

    #region Ctors

    public GetUsersResult(
        List<UserDto> items,
        long totalCount,
        PaginationRequest pagination)
    {
        Items = items;
        Paging = PagingResult.Of(totalCount, pagination);
    }

    #endregion
}

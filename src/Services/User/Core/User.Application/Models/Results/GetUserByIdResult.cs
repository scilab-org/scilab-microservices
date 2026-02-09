#region using

using User.Application.Dtos.Users;

#endregion

namespace User.Application.Models.Results;

public sealed class GetUserByIdResult
{
    #region Fields, Properties and Indexers

    public UserDto User { get; init; }

    #endregion

    #region Ctors

    public GetUserByIdResult(UserDto user)
    {
        User = user;
    }

    #endregion
}

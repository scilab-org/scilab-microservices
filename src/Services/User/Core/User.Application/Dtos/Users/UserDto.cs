#region using

using User.Application.Dtos.Groups;
using User.Application.Dtos.Roles;

#endregion

namespace User.Application.Dtos.Users;

public sealed class UserDto
{
    #region Fields, Properties and Indexers

    public string Id { get; set; } = default!;

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; }

    public bool EmailVerified { get; set; }

    public long CreatedTimestamp { get; set; }

    public List<GroupDto> Groups { get; set; } = [];

    #endregion
}

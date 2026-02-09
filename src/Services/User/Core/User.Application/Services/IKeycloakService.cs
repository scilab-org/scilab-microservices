#region using

using User.Application.Dtos.Groups;
using User.Application.Dtos.Roles;
using User.Application.Dtos.Users;

#endregion

namespace User.Application.Services;

public interface IKeycloakService
{
    #region Users

    Task<string> CreateUserAsync(
        string username,
        string email,
        string? firstName,
        string? lastName,
        string initialPassword,
        bool temporaryPassword = true,
        List<string>? groupNames = null,
        CancellationToken cancellationToken = default);

    Task UpdateUserAsync(
        string userId,
        string? firstName,
        string? lastName,
        bool? enabled,
        List<string>? groupNames,
        CancellationToken cancellationToken = default);

    Task<(List<UserDto> Users, int TotalCount)> GetUsersAsync(
        string? searchText,
        string? groupName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserDto> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task DeactivateUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    #endregion

    #region Groups

    Task<List<GroupDto>> GetGroupsAsync(
        CancellationToken cancellationToken = default);

    #endregion

    #region Roles

    Task<List<RoleDto>> GetRealmRolesAsync(
        CancellationToken cancellationToken = default);

    Task<List<RoleDto>> GetGroupRolesAsync(
        string groupId,
        CancellationToken cancellationToken = default);

    Task AddRolesToGroupAsync(
        string groupId,
        List<string> roleNames,
        CancellationToken cancellationToken = default);

    Task RemoveRolesFromGroupAsync(
        string groupId,
        List<string> roleNames,
        CancellationToken cancellationToken = default);

    #endregion
}

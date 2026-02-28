using Management.Application.Dtos.Members;
namespace Management.Application.Services;
public interface IUserApiService
{
    #region Methods
    /// <summary>
    /// Checks whether all provided userIds exist in the User service.
    /// Returns only the IDs that are valid/existing.
    /// </summary>
    Task<List<Guid>> GetExistingUserIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Returns all users from User service that are NOT already members of the project
    /// and are NOT admins (i.e. not in the admin group).
    /// </summary>
    Task<List<UserInfoDto>> GetAvailableProjectUsersAsync(
        IEnumerable<Guid> existingMemberUserIds,
        string adminGroupName,
        string? searchText = null,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Fetches user details for the given set of userIds from the User service.
    /// </summary>
    Task<List<UserInfoDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Assigns a Keycloak group to the user based on the project role.
    /// Calls User service PUT /users/{userId} with groupNames to update Keycloak.
    /// </summary>
    Task AssignUserRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default);
    #endregion
}
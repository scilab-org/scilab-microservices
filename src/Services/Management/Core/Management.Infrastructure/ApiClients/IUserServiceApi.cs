using Refit;

namespace Management.Infrastructure.ApiClients;

public interface IUserServiceApi
{
    #region Users

    /// <summary>
    /// GET /users/{userId} — returns 200 if user exists, 404 if not.
    /// </summary>
    [Get("/users/{userId}")]
    Task<HttpResponseMessage> GetUserByIdAsync([AliasAs("userId")] string userId);

    /// <summary>
    /// GET /users?pageNumber=1&pageSize=1000 — returns all users.
    /// </summary>
    [Get("/users")]
    Task<HttpResponseMessage> GetUsersAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000);

    /// <summary>
    /// PUT /users/{userId} — updates user profile including group assignments in Keycloak.
    /// </summary>
    [Multipart]
    [Put("/users/{userId}")]
    Task<HttpResponseMessage> UpdateUserAsync(
        [AliasAs("userId")] string userId,
        [AliasAs("FirstName")] string? firstName,
        [AliasAs("LastName")] string? lastName,
        [AliasAs("Enabled")] bool enabled,
        [AliasAs("GroupNames")] string? groupNames);

    /// <summary>
    /// PUT /users/{userId}/groups — updates only group memberships.
    /// </summary>
    [Put("/users/{userId}/groups")]
    Task<HttpResponseMessage> UpdateUserGroupsAsync(
        [AliasAs("userId")] string userId,
        [Body] UpdateUserGroupsRequest request);

    #endregion
}

/// <summary>
/// <summary>
/// Multipart update payload for User service.
/// </summary>
public sealed class UpdateUserGroupRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Enabled { get; set; } = true;
    public string? GroupNames { get; set; }
}

public sealed class UpdateUserGroupsRequest
{
    public List<string> GroupNames { get; set; } = [];
}


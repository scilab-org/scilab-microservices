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
    [Put("/users/{userId}")]
    Task<HttpResponseMessage> UpdateUserAsync(
        [AliasAs("userId")] string userId,
        [Body] UpdateUserGroupRequest request);

    #endregion
}

/// <summary>
/// Request body matching User service's UpdateUserDto shape.
/// Only groupNames is required for role assignment; other fields are optional.
/// </summary>
public sealed class UpdateUserGroupRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Enabled { get; set; } = true;
    public List<string>? GroupNames { get; set; }
}


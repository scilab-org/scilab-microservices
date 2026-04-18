using Refit;

namespace Lab.Infrastructure.ApiClients;

public interface IUserServiceApi
{
    /// <summary>
    /// GET /users/{userId} — returns a single user by ID.
    /// </summary>
    [Get("/users/{userId}")]
    Task<HttpResponseMessage> GetUserByIdAsync([AliasAs("userId")] string userId);

    /// <summary>
    /// GET /users?pageNumber=1&pageSize=1000 — returns all users (paginated).
    /// </summary>
    [Get("/users")]
    Task<HttpResponseMessage> GetUsersAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000,
        [AliasAs("enabled")] bool? enabled = true);
}


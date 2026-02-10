using Refit;
using User.Application.Models.Requests.Externals;
using User.Application.Models.Responses.Externals;

namespace User.Infrastructure.ApiClients;

public interface IKeycloakApi
{
    #region Token

    [Post("/realms/{realm}/protocol/openid-connect/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<KeycloakAccessTokenResponse> GetAccessTokenAsync(
        [AliasAs("realm")] string realm,
        [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> form);

    #endregion

    #region Users

    [Post("/admin/realms/{realm}/users")]
    Task<HttpResponseMessage> CreateUserAsync(
        [AliasAs("realm")] string realm,
        [Body] KeycloakCreateUserRequest request,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users")]
    Task<List<KeycloakUserResponse>> GetUsersAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("username")] string? username,
        [AliasAs("exact")] bool exact,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users")]
    Task<List<KeycloakUserResponse>> SearchUsersAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("search")] string? search,
        [AliasAs("first")] int first,
        [AliasAs("max")] int max,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users/count")]
    Task<int> GetUsersCountAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("search")] string? search,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users/{userId}")]
    Task<KeycloakUserResponse> GetUserByIdAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Authorize("Bearer")] string token);

    [Put("/admin/realms/{realm}/users/{userId}")]
    Task UpdateUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] KeycloakUpdateUserRequest request,
        [Authorize("Bearer")] string token);

    [Put("/admin/realms/{realm}/users/{userId}/reset-password")]
    Task ResetPasswordAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Body] KeycloakResetPasswordRequest request,
        [Authorize("Bearer")] string token);

    [Delete("/admin/realms/{realm}/users/{userId}")]
    Task DeleteUserAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users/{userId}/groups")]
    Task<List<KeycloakGroupResponse>> GetUserGroupsAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/users/{userId}/role-mappings/realm")]
    Task<List<KeycloakRoleResponse>> GetUserRealmRoleMappingsAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [Authorize("Bearer")] string token);

    #endregion

    #region Groups

    [Get("/admin/realms/{realm}/groups")]
    Task<List<KeycloakGroupResponse>> GetGroupsAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("search")] string? search,
        [Authorize("Bearer")] string token);

    [Put("/admin/realms/{realm}/users/{userId}/groups/{groupId}")]
    Task AssignUserToGroupAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [AliasAs("groupId")] string groupId,
        [Authorize("Bearer")] string token);

    [Delete("/admin/realms/{realm}/users/{userId}/groups/{groupId}")]
    Task RemoveUserFromGroupAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("userId")] string userId,
        [AliasAs("groupId")] string groupId,
        [Authorize("Bearer")] string token);

    #endregion

    #region Roles

    [Get("/admin/realms/{realm}/roles")]
    Task<List<KeycloakRoleResponse>> GetRealmRolesAsync(
        [AliasAs("realm")] string realm,
        [Authorize("Bearer")] string token);

    [Get("/admin/realms/{realm}/groups/{groupId}/role-mappings/realm")]
    Task<List<KeycloakRoleResponse>> GetGroupRealmRolesAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("groupId")] string groupId,
        [Authorize("Bearer")] string token);

    [Post("/admin/realms/{realm}/groups/{groupId}/role-mappings/realm")]
    Task AddRolesToGroupAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("groupId")] string groupId,
        [Body] List<KeycloakRoleResponse> roles,
        [Authorize("Bearer")] string token);

    [Delete("/admin/realms/{realm}/groups/{groupId}/role-mappings/realm")]
    Task RemoveRolesFromGroupAsync(
        [AliasAs("realm")] string realm,
        [AliasAs("groupId")] string groupId,
        [Body] List<KeycloakRoleResponse> roles,
        [Authorize("Bearer")] string token);

    #endregion
}
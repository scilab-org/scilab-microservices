#region using

using System.Net;
using Common.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using User.Application.Dtos.Groups;
using User.Application.Dtos.Roles;
using User.Application.Dtos.Users;
using User.Application.Models.Requests.Externals;
using User.Application.Models.Responses.Externals;
using User.Application.Services;
using User.Infrastructure.ApiClients;

#endregion

namespace User.Infrastructure.Services;

public sealed class KeycloakService : IKeycloakService
{
    #region Fields, Properties and Indexers

    private readonly IKeycloakApi _keycloakApi;
    private readonly IConfiguration _cfg;
    private readonly ILogger<KeycloakService> _logger;

    private readonly string _realm;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _grantType;
    private readonly string[] _scopes;

    #endregion

    #region Ctors

    public KeycloakService(
        IKeycloakApi keycloakApi,
        IConfiguration cfg,
        ILogger<KeycloakService> logger)
    {
        _keycloakApi = keycloakApi;
        _cfg = cfg;
        _logger = logger;
        _realm = cfg[$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Realm}"]!;
        _clientId = cfg[$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientId}"]!;
        _clientSecret = cfg[$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.ClientSecret}"]!;
        _grantType = cfg[$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.GrantType}"]!;
        _scopes = cfg.GetRequiredSection($"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Scopes}")
            .Get<string[]>() ?? throw new ArgumentNullException($"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.Scopes}");
    }

    #endregion

    #region Implementations

    public async Task<string> CreateUserAsync(
        string username,
        string email,
        string? firstName,
        string? lastName,
        string initialPassword,
        bool temporaryPassword = true,
        List<string>? groupNames = null,
        CancellationToken cancellationToken = default)
    {
        string? createdUserId = null;
        string? accessToken = null;

        try
        {
            // Step 1: Get access token
            accessToken = await GetAccessTokenAsync();

            // Step 2: Create user in Keycloak
            var createUserRequest = new KeycloakCreateUserRequest
            {
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Enabled = true,
                EmailVerified = false,
                Credentials =
                [
                    new KeycloakCredential
                    {
                        Type = "password",
                        Value = initialPassword,
                        Temporary = temporaryPassword
                    }
                ]
            };

            var response = await _keycloakApi.CreateUserAsync(_realm, createUserRequest, accessToken);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new InfrastructureException(MessageCode.UserAlreadyExists);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to create user in Keycloak. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                throw new InfrastructureException(MessageCode.FailedToCreateUser);
            }

            _logger.LogInformation("Successfully created user '{Username}' in Keycloak", username);

            // Step 3: Get the created user's ID
            createdUserId = await GetUserIdByUsernameAsync(username, accessToken);

            // Step 4: Assign groups
            if (groupNames is { Count: > 0 })
            {
                await AssignGroupsToUserAsync(createdUserId, groupNames, accessToken);
            }

            return createdUserId;
        }
        catch (InfrastructureException infrastructureEx)
        {
            // Compensation: If user was created but something failed afterwards, delete the user
            if (!string.IsNullOrEmpty(createdUserId) && !string.IsNullOrEmpty(accessToken))
            {
                await CompensateUserCreationAsync(createdUserId, accessToken);
            }

            throw;
        }
        catch (Exception ex)
        {
            // Compensation: If user was created but something failed afterwards, delete the user
            if (!string.IsNullOrEmpty(createdUserId) && !string.IsNullOrEmpty(accessToken))
            {
                await CompensateUserCreationAsync(createdUserId, accessToken);
            }

            _logger.LogError(ex, "Unexpected error during user creation for '{Username}'", username);
            throw new InfrastructureException(MessageCode.UnknownError);
        }
    }

    public async Task UpdateUserAsync(
        string userId,
        string? firstName,
        string? lastName,
        bool? enabled,
        List<string>? groupNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new KeycloakUpdateUserRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Enabled = enabled
            };

            await _keycloakApi.UpdateUserAsync(_realm, userId, request, accessToken);

            // Update group memberships if provided
            if (groupNames is not null)
            {
                await SyncUserGroupsAsync(userId, groupNames, accessToken);
            }

            _logger.LogInformation("Successfully updated user '{UserId}' in Keycloak", userId);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("User '{UserId}' not found in Keycloak", userId);
            throw new InfrastructureException(MessageCode.UserNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to update user '{UserId}' in Keycloak", userId);
            throw new InfrastructureException(MessageCode.FailedToUpdateUser);
        }
    }

    public async Task<(List<UserDto> Users, int TotalCount)> GetUsersAsync(
        string? searchText,
        string? groupName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();

            var totalCount = await _keycloakApi.GetUsersCountAsync(_realm, searchText, accessToken);

            var first = (pageNumber - 1) * pageSize;
            var users = await _keycloakApi.SearchUsersAsync(_realm, searchText, first, pageSize, accessToken);

            // Enrich each user with groups and realm roles
            var enrichedUsers = new List<UserDto>();
            foreach (var user in users)
            {
                var userGroups = await _keycloakApi.GetUserGroupsAsync(_realm, user.Id, accessToken);   

                enrichedUsers.Add(MapToUserDto(user, userGroups));
            }

            // Apply client-side filtering by group name
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                enrichedUsers = enrichedUsers
                    .Where(u => u.Groups.Any(g =>
                        g.Name != null && g.Name.Contains(groupName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Adjust total count if client-side filters were applied
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                totalCount = enrichedUsers.Count;
            }

            return (enrichedUsers, totalCount);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to get users from Keycloak");
            throw new InfrastructureException(MessageCode.FailedToGetUsers);
        }
    }

    public async Task<UserDto> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var user = await _keycloakApi.GetUserByIdAsync(_realm, userId, accessToken);

            var userGroups = await _keycloakApi.GetUserGroupsAsync(_realm, userId, accessToken);
            var userRoles = await _keycloakApi.GetUserRealmRoleMappingsAsync(_realm, userId, accessToken);

            return MapToUserDto(user, userGroups);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("User '{UserId}' not found in Keycloak", userId);
            throw new InfrastructureException(MessageCode.UserNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to get user '{UserId}' from Keycloak", userId);
            throw new InfrastructureException(MessageCode.FailedToRetrieveUser);
        }
    }

    public async Task DeactivateUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();

            var request = new KeycloakUpdateUserRequest
            {
                Enabled = false
            };

            await _keycloakApi.UpdateUserAsync(_realm, userId, request, accessToken);
            _logger.LogInformation("Successfully deactivated user '{UserId}' in Keycloak", userId);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("User '{UserId}' not found in Keycloak", userId);
            throw new InfrastructureException(MessageCode.UserNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to deactivate user '{UserId}' in Keycloak", userId);
            throw new InfrastructureException(MessageCode.FailedToDeactivateUser);
        }
    }

    public async Task<List<GroupDto>> GetGroupsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var groups = await _keycloakApi.GetGroupsAsync(_realm, search: null, accessToken);

            return groups.Select(MapToGroupDto).ToList();
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to get groups from Keycloak");
            throw new InfrastructureException(MessageCode.FailedToGetGroups);
        }
    }

    public async Task<List<RoleDto>> GetRealmRolesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var roles = await _keycloakApi.GetRealmRolesAsync(_realm, accessToken);

            return roles.Select(MapToRoleDto).ToList();
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to get realm roles from Keycloak");
            throw new InfrastructureException(MessageCode.FailedToGetRoles);
        }
    }

    public async Task<List<RoleDto>> GetGroupRolesAsync(
        string groupId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var roles = await _keycloakApi.GetGroupRealmRolesAsync(_realm, groupId, accessToken);

            return roles.Select(MapToRoleDto).ToList();
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("Group '{GroupId}' not found in Keycloak", groupId);
            throw new InfrastructureException(MessageCode.GroupNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to get roles for group '{GroupId}'", groupId);
            throw new InfrastructureException(MessageCode.FailedToGetGroupRoles);
        }
    }

    public async Task AddRolesToGroupAsync(
        string groupId,
        List<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var allRoles = await _keycloakApi.GetRealmRolesAsync(_realm, accessToken);

            var rolesToAdd = ResolveRolesByName(allRoles, roleNames);

            await _keycloakApi.AddRolesToGroupAsync(_realm, groupId, rolesToAdd, accessToken);
            _logger.LogInformation("Added roles [{Roles}] to group '{GroupId}'",
                string.Join(", ", roleNames), groupId);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("Group '{GroupId}' not found in Keycloak", groupId);
            throw new InfrastructureException(MessageCode.GroupNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to add roles to group '{GroupId}'", groupId);
            throw new InfrastructureException(MessageCode.FailedToAddRoleToGroup);
        }
    }

    public async Task RemoveRolesFromGroupAsync(
        string groupId,
        List<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var accessToken = await GetAccessTokenAsync();
            var allRoles = await _keycloakApi.GetRealmRolesAsync(_realm, accessToken);

            var rolesToRemove = ResolveRolesByName(allRoles, roleNames);

            await _keycloakApi.RemoveRolesFromGroupAsync(_realm, groupId, rolesToRemove, accessToken);
            _logger.LogInformation("Removed roles [{Roles}] from group '{GroupId}'",
                string.Join(", ", roleNames), groupId);
        }
        catch (Refit.ApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogError("Group '{GroupId}' not found in Keycloak", groupId);
            throw new InfrastructureException(MessageCode.GroupNotFound);
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to remove roles from group '{GroupId}'", groupId);
            throw new InfrastructureException(MessageCode.FailedToRemoveRoleFromGroup);
        }
    }

    #endregion

    #region Private Methods

    private async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var form = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "client_secret", _clientSecret },
                { "grant_type", _grantType },
                { "scope", string.Join(" ", _scopes) }
            };

            var tokenResponse = await _keycloakApi.GetAccessTokenAsync(_realm, form);

            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                _logger.LogError("Access token is null or empty from Keycloak");
                throw new InfrastructureException(MessageCode.FailedToGetAccessToken);
            }

            return tokenResponse.AccessToken;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Failed to obtain access token from Keycloak");
            throw new InfrastructureException(MessageCode.FailedToGetAccessToken);
        }
    }

    private async Task<string> GetUserIdByUsernameAsync(string username, string accessToken)
    {
        try
        {
            var users = await _keycloakApi.GetUsersAsync(_realm, username, exact: true, accessToken);

            var user = users.FirstOrDefault();
            if (user is null)
            {
                _logger.LogError("User '{Username}' was created but could not be retrieved from Keycloak", username);
                throw new InfrastructureException(MessageCode.FailedToRetrieveUser);
            }

            return user.Id;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Error retrieving user '{Username}' from Keycloak", username);
            throw new InfrastructureException(MessageCode.FailedToRetrieveUser);
        }
    }

    private async Task AssignGroupsToUserAsync(string userId, List<string> groupNames, string accessToken)
    {
        try
        {
            var allGroups = await _keycloakApi.GetGroupsAsync(_realm, search: null, accessToken);

            foreach (var groupName in groupNames)
            {
                var group = FindGroupByName(allGroups, groupName);
                if (group is null)
                {
                    _logger.LogError("Group '{GroupName}' not found in Keycloak realm '{Realm}'", groupName, _realm);
                    throw new InfrastructureException(MessageCode.GroupNotFound);
                }

                try
                {
                    await _keycloakApi.AssignUserToGroupAsync(_realm, userId, group.Id, accessToken);
                    _logger.LogInformation("Assigned user '{UserId}' to group '{GroupName}'", userId, groupName);
                }
                catch (Exception ex) when (ex is not InfrastructureException)
                {
                    _logger.LogError(ex, "Failed to assign user '{UserId}' to group '{GroupName}'", userId, groupName);
                    throw new InfrastructureException(MessageCode.FailedToAssignGroup);
                }
            }
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            _logger.LogError(ex, "Error during group assignment for user '{UserId}'", userId);
            throw new InfrastructureException(MessageCode.FailedToAssignGroup);
        }
    }

    private async Task CompensateUserCreationAsync(string userId, string accessToken)
    {
        try
        {
            await _keycloakApi.DeleteUserAsync(_realm, userId, accessToken);
            _logger.LogInformation(
                "Successfully compensated: Deleted user '{UserId}' due to creation failure", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Compensation failed: Could not delete user '{UserId}' after creation failure. Please delete manually.",
                userId);
            throw new InfrastructureException(MessageCode.UserCreationCompensationFailed);
        }
    }

    private static Application.Models.Responses.Externals.KeycloakGroupResponse? FindGroupByName(
        List<Application.Models.Responses.Externals.KeycloakGroupResponse> groups,
        string groupName)
    {
        foreach (var group in groups)
        {
            if (string.Equals(group.Name, groupName, StringComparison.OrdinalIgnoreCase))
                return group;

            if (group.SubGroups is { Count: > 0 })
            {
                var found = FindGroupByName(group.SubGroups, groupName);
                if (found is not null) return found;
            }
        }

        return null;
    }

    private static UserDto MapToUserDto(
        KeycloakUserResponse user,
        List<KeycloakGroupResponse>? groups = null) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Enabled = user.Enabled,
        EmailVerified = user.EmailVerified,
        CreatedTimestamp = user.CreatedTimestamp,
        Groups = groups?.Select(MapToGroupDto).ToList() ?? [],
    };

    private static GroupDto MapToGroupDto(KeycloakGroupResponse group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        Path = group.Path,
        SubGroups = group.SubGroups?.Select(MapToGroupDto).ToList()
    };

    private static RoleDto MapToRoleDto(KeycloakRoleResponse role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description,
        Composite = role.Composite,
        ClientRole = role.ClientRole
    };

    private static List<KeycloakRoleResponse> ResolveRolesByName(
        List<KeycloakRoleResponse> allRoles,
        List<string> roleNames)
    {
        var resolved = new List<KeycloakRoleResponse>();

        foreach (var roleName in roleNames)
        {
            var role = allRoles.FirstOrDefault(r =>
                string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));

            if (role is null)
                throw new InfrastructureException(MessageCode.RoleNotFound);

            resolved.Add(role);
        }

        return resolved;
    }

    private async Task SyncUserGroupsAsync(string userId, List<string> desiredGroupNames, string accessToken)
    {
        var currentGroups = await _keycloakApi.GetUserGroupsAsync(_realm, userId, accessToken);
        var allGroups = await _keycloakApi.GetGroupsAsync(_realm, search: null, accessToken);

        var currentGroupNames = currentGroups
            .Select(g => g.Name)
            .Where(n => n is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var desiredSet = desiredGroupNames.ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Remove user from groups not in the desired list
        foreach (var currentGroup in currentGroups)
        {
            if (currentGroup.Name is not null && !desiredSet.Contains(currentGroup.Name))
            {
                await _keycloakApi.RemoveUserFromGroupAsync(_realm, userId, currentGroup.Id, accessToken);
                _logger.LogInformation("Removed user '{UserId}' from group '{GroupName}'", userId, currentGroup.Name);
            }
        }

        // Add user to groups not currently assigned
        foreach (var groupName in desiredGroupNames)
        {
            if (!currentGroupNames.Contains(groupName))
            {
                var group = FindGroupByName(allGroups, groupName);
                if (group is null)
                {
                    _logger.LogError("Group '{GroupName}' not found in Keycloak realm '{Realm}'", groupName, _realm);
                    throw new InfrastructureException(MessageCode.GroupNotFound);
                }

                await _keycloakApi.AssignUserToGroupAsync(_realm, userId, group.Id, accessToken);
                _logger.LogInformation("Assigned user '{UserId}' to group '{GroupName}'", userId, groupName);
            }
        }
    }

    #endregion
}

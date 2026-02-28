using System.Net.Http.Json;
using Common.Constants;
using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Infrastructure.ApiClients;

namespace Management.Infrastructure.Services;

// Internal shape matching User service response
file sealed class UserServiceItem
{
    public string Id { get; set; } = default!;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Enabled { get; set; }
    public List<UserServiceGroup>? Groups { get; set; }
}

file sealed class UserServiceGroup
{
    public string? Name { get; set; }
}

file sealed class UserServiceResult
{
    public List<UserServiceItem> Items { get; set; } = new();
}

file sealed class UserServiceGetResponse
{
    public UserServiceResult? Result { get; set; }
}

file sealed class UserServiceGetByIdResult
{
    public UserServiceItem? User { get; set; }
}

file sealed class UserServiceGetByIdResponse
{
    public UserServiceGetByIdResult? Result { get; set; }
}

public sealed class UserApiService(IUserServiceApi userServiceApi) : IUserApiService
{
    #region Implementations

    public async Task<List<Guid>> GetExistingUserIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var validIds = new List<Guid>();

        foreach (var userId in userIds)
        {
            try
            {
                var response = await userServiceApi.GetUserByIdAsync(userId.ToString());
                if (response.IsSuccessStatusCode)
                    validIds.Add(userId);
            }
            catch
            {
                // If user service is unreachable or returns an error, skip this userId
            }
        }

        return validIds;
    }

    public async Task<List<UserInfoDto>> GetAvailableProjectUsersAsync(
        IEnumerable<Guid> existingMemberUserIds,
        string adminGroupName,
        string? searchText = null,
        CancellationToken cancellationToken = default)
    {
        var memberSet = existingMemberUserIds.Select(x => x.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var response = await userServiceApi.GetUsersAsync(pageNumber: 1, pageSize: 1000);
        if (!response.IsSuccessStatusCode)
            return new List<UserInfoDto>();

        var body = await response.Content.ReadFromJsonAsync<UserServiceGetResponse>(
            cancellationToken: cancellationToken);

        var allUsers = body?.Result?.Items ?? new List<UserServiceItem>();

        return allUsers
            .Where(u =>
                // Exclude already-members
                !memberSet.Contains(u.Id) &&
                // Exclude admins
                !(u.Groups?.Any(g => string.Equals(g.Name, adminGroupName, StringComparison.OrdinalIgnoreCase)) ?? false) &&
                // Optional search
                (string.IsNullOrWhiteSpace(searchText) ||
                 (u.Username?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (u.Email?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (u.FirstName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                 (u.LastName?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false)))
            .Select(u => new UserInfoDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Enabled = u.Enabled
            })
            .ToList();
    }

    public async Task<List<UserInfoDto>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var idSet = userIds.Select(x => x.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (idSet.Count == 0) return new List<UserInfoDto>();

        var response = await userServiceApi.GetUsersAsync(pageNumber: 1, pageSize: 1000);
        if (!response.IsSuccessStatusCode)
            return new List<UserInfoDto>();

        var body = await response.Content.ReadFromJsonAsync<UserServiceGetResponse>(
            cancellationToken: cancellationToken);

        var allUsers = body?.Result?.Items ?? new List<UserServiceItem>();

        return allUsers
            .Where(u => idSet.Contains(u.Id))
            .Select(u => new UserInfoDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Enabled = u.Enabled,
                Groups = u.Groups?.Select(g => g.Name).Where(n => n is not null).ToList()! ?? new List<string>()
            })
            .ToList();
    }

    public async Task AssignUserRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        // First get current user info so we don't overwrite existing fields
        var userResponse = await userServiceApi.GetUserByIdAsync(userId.ToString());
        if (!userResponse.IsSuccessStatusCode)
            throw new InfrastructureException(MessageCode.UserNotFound);

        var userBody = await userResponse.Content.ReadFromJsonAsync<UserServiceGetByIdResponse>(
            cancellationToken: cancellationToken);

        var user = userBody?.Result?.User;

        // Build update request: keep existing info, add the new group (role)
        var currentGroups = user?.Groups?.Select(g => g.Name).Where(n => n is not null).ToList()
                            ?? new List<string?>();

        // Add the new role group if not already present
        if (!currentGroups.Any(g => string.Equals(g, roleName, StringComparison.OrdinalIgnoreCase)))
        {
            currentGroups.Add(roleName);
        }

        var updateRequest = new UpdateUserGroupRequest
        {
            FirstName = user?.FirstName,
            LastName = user?.LastName,
            Enabled = user?.Enabled ?? true,
            GroupNames = currentGroups!
        };

        var updateResponse = await userServiceApi.UpdateUserAsync(userId.ToString(), updateRequest);
        if (!updateResponse.IsSuccessStatusCode)
            throw new InfrastructureException(MessageCode.FailedToAssignGroup);
    }

    #endregion
}


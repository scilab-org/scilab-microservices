using System.Net.Http.Json;
using Lab.Application.Services;
using Lab.Infrastructure.ApiClients;

namespace Lab.Infrastructure.Services;

public sealed class UserApiService(IUserServiceApi userServiceApi) : IUserApiService
{
    public async Task<Dictionary<Guid, UserInfo>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default)
    {
        var idSet = userIds.Select(x => x.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (idSet.Count == 0) return [];

        var response = await userServiceApi.GetUsersAsync(pageNumber: 1, pageSize: 1000);
        if (!response.IsSuccessStatusCode)
            return [];

        var body = await response.Content
            .ReadFromJsonAsync<UserServiceGetResponse>(cancellationToken: cancellationToken);

        var allUsers = body?.Result?.Items ?? [];

        return allUsers
            .Where(u => idSet.Contains(u.Id))
            .ToDictionary(
                u => Guid.Parse(u.Id),
                u => new UserInfo(Guid.Parse(u.Id), u.Username, u.Email, u.FirstName, u.LastName));
    }
}

// Internal shapes matching User service response
file sealed class UserServiceItem
{
    public string Id { get; set; } = default!;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

file sealed class UserServiceResult
{
    public List<UserServiceItem> Items { get; set; } = [];
}

file sealed class UserServiceGetResponse
{
    public UserServiceResult? Result { get; set; }
}


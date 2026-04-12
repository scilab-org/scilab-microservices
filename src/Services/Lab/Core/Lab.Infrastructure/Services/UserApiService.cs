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
        var ids = userIds.ToList();
        if (ids.Count == 0) return [];

        var result = new Dictionary<Guid, UserInfo>(ids.Count);

        foreach (var userId in ids)
        {
            try
            {
                var response = await userServiceApi.GetUserByIdAsync(userId.ToString());
                if (!response.IsSuccessStatusCode)
                    continue;

                var body = await response.Content
                    .ReadFromJsonAsync<UserServiceGetByIdResponse>(cancellationToken: cancellationToken);

                var u = body?.Result?.User;
                if (u is null)
                    continue;

                result[userId] = new UserInfo(Guid.Parse(u.Id), u.Username, u.Email, u.FirstName, u.LastName);
            }
            catch
            {
                // skip unreachable / errored entries
            }
        }

        return result;
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

file sealed class UserServiceGetByIdResult
{
    public UserServiceItem? User { get; set; }
}

file sealed class UserServiceGetByIdResponse
{
    public UserServiceGetByIdResult? Result { get; set; }
}


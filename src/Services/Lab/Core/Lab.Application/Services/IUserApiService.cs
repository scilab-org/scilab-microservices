namespace Lab.Application.Services;

public interface IUserApiService
{
    /// <summary>
    /// Returns user info (Name, Email) for a list of user IDs.
    /// Returns a dictionary of UserId -> UserInfo.
    /// </summary>
    Task<Dictionary<Guid, UserInfo>> GetUsersByIdsAsync(
        IEnumerable<Guid> userIds,
        CancellationToken cancellationToken = default);
}

public sealed record UserInfo(
    Guid Id,
    string? Username,
    string? Email,
    string? FirstName,
    string? LastName);


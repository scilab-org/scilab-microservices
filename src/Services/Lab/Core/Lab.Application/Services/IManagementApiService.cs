namespace Lab.Application.Services;

public interface IManagementApiService
{
    /// <summary>
    /// Calls the Management service to fetch a project by Id.
    /// Returns null when project cannot be resolved.
    /// </summary>
    Task<ManagementProjectInfo?> GetProjectByIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls the Management service to create a sub-project under the given project,
    /// linking it to the specified paper.
    /// Returns the newly created sub-project Id.
    /// </summary>
    Task<Guid?> CreateSubProjectAsync(
        Guid projectId,
        Guid paperId,
        string? name = "",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Single call that resolves the sub-project from paperId and returns the
    /// memberId + subProjectId for the given user — replaces the two separate calls.
    /// </summary>
    Task<(Guid SubProjectId, Guid MemberId)?> GetMemberByPaperIdAsync(
        Guid paperId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all members of the sub-project that owns the given paper.
    /// Each entry contains (MemberId, UserId, Role).
    /// </summary>
    Task<List<SubProjectMemberInfo>> GetSubProjectMembersByPaperIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves UserId for a list of MemberIds by fetching member info from Management.
    /// Returns a dictionary of MemberId -> UserId.
    /// </summary>
    Task<Dictionary<Guid, Guid>> GetUserIdsByMemberIdsAsync(
        Guid paperId,
        IEnumerable<Guid> memberIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls the Management service to get the current user's role in the given project.
    /// Returns the role name string, or null if the call fails.
    /// </summary>
    Task<string?> GetMyProjectRoleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
}

public sealed record SubProjectMemberInfo(
    Guid MemberId,
    Guid UserId,
    string Role,
    string? Username = null,
    string? Email = null,
    string? FirstName = null,
    string? LastName = null);

public sealed record ManagementProjectInfo(
    Guid Id,
    string? Name,
    string? Code,
    string? Description,
    string? Status,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? Context,
    string? Domain,
    string? Keypoint);
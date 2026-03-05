namespace Lab.Application.Services;

public interface IManagementApiService
{
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
    /// Calls the Management service to get the current user's role in the given project.
    /// Returns the role name string, or null if the call fails.
    /// </summary>
    Task<string?> GetMyProjectRoleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
}
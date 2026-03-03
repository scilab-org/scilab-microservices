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
}
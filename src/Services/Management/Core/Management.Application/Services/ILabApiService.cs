using Management.Application.Dtos.Papers;

namespace Management.Application.Services;

public interface ILabApiService
{
    #region Methods

    /// <summary>
    /// Returns all papers from Lab service that are NOT already added to the project,
    /// with optional title search text.
    /// </summary>
    Task<List<PaperInfoDto>> GetAvailablePapersAsync(
        IEnumerable<Guid> existingPaperIds,
        string? searchText = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches paper details for the given set of paperIds from the Lab service.
    /// </summary>
    Task<List<PaperInfoDto>> GetPapersByIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Checks whether all provided paperIds exist in the Lab service.
    /// Returns only the IDs that are valid/existing.
    /// </summary>
    Task<List<Guid>> GetExistingPaperIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default);
    #endregion
}

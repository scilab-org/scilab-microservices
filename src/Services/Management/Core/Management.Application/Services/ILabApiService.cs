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

    /// <summary>
    /// Fetches paper details for the given set of paperIds from the Lab service,
    /// with optional title search and paging applied.
    /// </summary>
    Task<(List<PaperInfoDto> Items, long TotalCount)> GetPapersByIdsPagedAsync(
        IEnumerable<Guid> paperIds,
        string? title = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a paper by its ID from the Lab service.
    /// </summary>
    Task<bool> DeletePaperAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns all sections for a given paperId from the Lab service.</summary>
    Task<List<LabSectionDto>> GetSectionsByPaperIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a paper contributor record in the Lab service.</summary>
    Task<bool> CreatePaperContributorAsync(
        string sectionRole,
        Guid paperId,
        Guid memberId,
        Guid markSectionId,
        Guid? sectionId = null,
        CancellationToken cancellationToken = default);

    #endregion
}

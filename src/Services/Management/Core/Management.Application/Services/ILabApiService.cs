using Management.Application.Dtos.Dashboard;
using Management.Application.Dtos.Papers;

namespace Management.Application.Services;

public interface ILabApiService
{
    #region Methods

    /// <summary>
    /// Returns all paper-banks from Lab service that are NOT already added to the project,
    /// with optional filters matching the GetPaperBanks endpoint (minus IsDeleted).
    /// </summary>
    Task<(List<PaperBankInfoDto> Items, long TotalCount)> GetAvailablePapersAsync(
        IEnumerable<Guid> existingPaperIds,
        string? title = null,
        string[]? author = null,
        string? publisher = null,
        string? @abstract = null,
        string? doi = null,
        int? status = null,
        DateTimeOffset? fromPublicationDate = null,
        DateTimeOffset? toPublicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? conferenceName = null,
        string[]? keywords = null,
        int pageNumber = 1,
        int pageSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a single paper (PaperEntity) by its ID from the Lab service via GET /papers/{id}.
    /// Returns null if the paper does not exist or the service is unreachable.
    /// </summary>
    Task<PaperInfoDto?> GetPaperByIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches a single paper-bank entry by its ID from the Lab service via GET /paper-bank/{id}.
    /// Returns null if the entry does not exist or the service is unreachable.
    /// </summary>
    Task<PaperBankInfoDto?> GetPaperBankByIdAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches paper details for the given set of paperIds from the Lab service.
    /// </summary>
    Task<List<PaperBankInfoDto>> GetPaperBanksByIdsAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Checks whether all provided paperIds exist in the Lab service.
    /// Returns only the IDs that are valid/existing.
    /// </summary>
    Task<List<Guid>> GetExistingPaperBankIdsAsync(
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
    /// Fetches PaperBank details for the given set of paperIds from the Lab service,
    /// with optional title and keywords filters and paging applied (client-side).
    /// </summary>
    Task<(List<PaperBankInfoDto> Items, long TotalCount)> GetPaperBanksByIdsPagedAsync(
        IEnumerable<Guid> paperIds,
        string? title = null,
        string? pages = null,
        string? number = null,
        string? volume = null,
        string? referenceContent = null,
        string[]? keywords = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a paper bank by its ID from the Lab service.
    /// </summary>
    Task<bool> DeletePaperBankAsync(
        Guid paperId,
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

    /// <summary>Returns all paper authors for a given paperId from the Lab service.</summary>
    Task<List<LabPaperContributorDto>> GetPaperAuthorsAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns all paper contributors for a given paperId from the Lab service.</summary>
    Task<List<LabPaperContributorDto>> GetPaperContributorsAsync(
        Guid paperId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns all papers assigned to the current user across their projects.</summary>
    Task<(List<PaperInfoDto> Items, long TotalCount)> GetAssignedPapersAsync(
        string? title = null,
        string[]? author = null,
        string? publisher = null,
        string? @abstract = null,
        string? doi = null,
        int? status = null,
        DateTimeOffset? fromPublicationDate = null,
        DateTimeOffset? toPublicationDate = null,
        string? paperType = null,
        string? journalName = null,
        string? conferenceName = null,
        string[]? tag = null,
        int pageNumber = 1,
        int pageSize = 1000,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes a paper contributor by its id from the Lab service.</summary>
    Task<bool> DeletePaperContributorAsync(
        Guid contributorId,
        CancellationToken cancellationToken = default);

    /// <summary>Creates a paper contributor record in the Lab service.</summary>
    Task<bool> CreatePaperContributorAsync(
        string sectionRole,
        Guid paperId,
        List<Guid> memberIds,
        Guid markSectionId,
        Guid? sectionId = null,
        CancellationToken cancellationToken = default);

    /// <summary>Returns submission status counts for a batch of paper IDs in a single Lab DB query.</summary>
    Task<SubmissionStatusSummaryResult> GetSubmissionStatusSummaryAsync(
        IEnumerable<Guid> paperIds,
        CancellationToken cancellationToken = default);

    /// <summary>Rebuilds Lab section rules when a project's rule-related fields change.</summary>
    Task<bool> UpdateProjectRulesAsync(
        IEnumerable<Guid> paperIds,
        string? context,
        string? domain,
        string? keypoint,
        CancellationToken cancellationToken = default);

    /// <summary>Returns Lab-side admin dashboard KPIs (paper bank, journals, templates, submission status counts, recent papers).</summary>
    Task<LabAdminDashboardKpisDto> GetAdminDashboardKpisAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Returns Lab-side user dashboard KPIs (task counts, paper counts, recent tasks, recent papers).</summary>
    Task<LabUserDashboardKpisDto> GetUserDashboardKpisAsync(
        string username,
        IEnumerable<Guid> memberIds,
        CancellationToken cancellationToken = default);

    #endregion
}
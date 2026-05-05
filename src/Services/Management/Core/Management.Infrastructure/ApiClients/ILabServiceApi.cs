using Refit;

namespace Management.Infrastructure.ApiClients;

public interface ILabServiceApi
{
    #region Papers

    /// <summary>
    /// GET /papers — returns all papers (paged).
    /// </summary>
    [Get("/papers/sample")]
    Task<HttpResponseMessage> GetPapersSampleAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000,
        [AliasAs("title")] string? title = null);

    /// <summary>
    /// GET /paper-bank — returns paper banks with filters and paging.
    /// </summary>
    [Get("/paper-bank")]
    Task<HttpResponseMessage> GetPaperBanksAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000,
        [AliasAs("title")] string? title = null,
        [AliasAs("author"), Query(CollectionFormat.Multi)] string[]? author = null,
        [AliasAs("publisher")] string? publisher = null,
        [AliasAs("abstract")] string? @abstract = null,
        [AliasAs("doi")] string? doi = null,
        [AliasAs("status")] int? status = null,
        [AliasAs("fromPublicationDate")] DateTimeOffset? fromPublicationDate = null,
        [AliasAs("toPublicationDate")] DateTimeOffset? toPublicationDate = null,
        [AliasAs("paperType")] string? paperType = null,
        [AliasAs("journalName")] string? journalName = null,
        [AliasAs("conferenceName")] string? conferenceName = null,
        [AliasAs("keyword"), Query(CollectionFormat.Multi)] string[]? keywords = null,
        [AliasAs("existingPaperIds"), Query(CollectionFormat.Multi)] Guid[]? existingPaperIds = null);

    /// <summary>
    /// GET /papers/{id} — returns a single paper (PaperEntity) by id.
    /// </summary>
    [Get("/papers/{id}")]
    Task<HttpResponseMessage> GetPaperByIdAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// GET /paper-bank/{id} — returns a single paper-bank entry by id.
    /// </summary>
    [Get("/paper-bank/{id}")]
    Task<HttpResponseMessage> GetPaperBankByIdAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// DELETE /admin/paper-bank/{id} — deletes a paper bank by id.
    /// </summary>
    [Delete("/admin/paper-bank/{id}")]
    Task<HttpResponseMessage> DeletePaperBankAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// DELETE /manager/papers/{id} — deletes a paper by id.
    /// </summary>
    [Delete("/manager/papers/{id}")]
    Task<HttpResponseMessage> DeletePaperAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// GET /papers/{id}/sections — returns all sections for a paper.
    /// </summary>
    [Get("/papers/{id}/sections")]
    Task<HttpResponseMessage> GetSectionsByPaperIdAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// GET /paper-authors/{paperId} — returns paper authors for a paper.
    /// </summary>
    [Get("/paper-authors")]
    Task<HttpResponseMessage> GetPaperAuthorsAsync([AliasAs("paperId")] Guid paperId);

    /// <summary>
    /// GET /paper-contributors/papers/{paperId}/contributors — returns contributors for a paper.
    /// </summary>
    [Get("/paper-contributors/papers/{paperId}/contributors")]
    Task<HttpResponseMessage> GetPaperContributorsAsync([AliasAs("paperId")] Guid paperId);

    /// <summary>
    /// GET /papers/assigned — returns papers assigned to current user.
    /// </summary>
    [Get("/papers/assigned")]
    Task<HttpResponseMessage> GetAssignedPapersAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000,
        [AliasAs("title")] string? title = null,
        [AliasAs("author"), Query(CollectionFormat.Multi)] string[]? author = null,
        [AliasAs("publisher")] string? publisher = null,
        [AliasAs("abstract")] string? @abstract = null,
        [AliasAs("doi")] string? doi = null,
        [AliasAs("status")] int? status = null,
        [AliasAs("fromPublicationDate")] DateTimeOffset? fromPublicationDate = null,
        [AliasAs("toPublicationDate")] DateTimeOffset? toPublicationDate = null,
        [AliasAs("paperType")] string? paperType = null,
        [AliasAs("journalName")] string? journalName = null,
        [AliasAs("conferenceName")] string? conferenceName = null,
        [AliasAs("tag"), Query(CollectionFormat.Multi)] string[]? tag = null);

    /// <summary>
    /// DELETE /paper-contributors/{id} — deletes a contributor by id.
    /// </summary>
    [Delete("/author/paper-contributors/{id}")]
    Task<HttpResponseMessage> DeletePaperContributorAsync([AliasAs("id")] Guid id);

    /// <summary>
    /// POST /papers/submission-status-summary — returns submission status counts for a batch of paper IDs.
    /// </summary>
    [Post("/papers/submission-status-summary")]
    Task<HttpResponseMessage> GetSubmissionStatusSummaryAsync([Body] LabSubmissionStatusSummaryRequest body);

    /// <summary>
    /// GET /admin/dashboard/kpis — returns Lab-side admin dashboard KPIs.
    /// </summary>
    [Get("/admin/dashboard/kpis")]
    Task<HttpResponseMessage> GetAdminDashboardKpisAsync();

    #endregion

    #region Paper Contributors

    /// <summary>
    /// POST /author/paper-contributors — creates a paper contributor record in Lab service.
    /// </summary>
    [Post("/author/paper-contributors")]
    Task<HttpResponseMessage> CreatePaperContributorAsync([Body] CreatePaperContributorRequest body);

    /// <summary>
    /// PUT /admin/system/project-rules — rebuilds project-derived rules for Lab sections.
    /// </summary>
    [Put("/admin/system/project-rules")]
    Task<HttpResponseMessage> UpdateProjectRulesAsync([Body] UpdateProjectRulesRequest body);

    #endregion
}

public sealed class CreatePaperContributorRequest
{
    public string SectionRole { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public List<Guid> MemberIds { get; set; } = [];
    public Guid MarkSectionId { get; set; }
}

public sealed class LabSubmissionStatusSummaryRequest
{
    public List<Guid> PaperIds { get; set; } = [];
}

public sealed class UpdateProjectRulesRequest
{
    public List<Guid> PaperIds { get; set; } = [];
    public string? Context { get; set; }
    public string? Domain { get; set; }
    public string? Keypoint { get; set; }
}

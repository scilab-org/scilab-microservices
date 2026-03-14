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
    /// GET /papers/{paperId} — returns a single paper by id.
    /// </summary>
    [Get("/paper-bank/{id}")]
    Task<HttpResponseMessage> GetPaperByIdAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// DELETE /admin/paper-bank/{id} — deletes a paper by id.
    /// </summary>
    [Delete("/admin/paper-bank/{id}")]
    Task<HttpResponseMessage> DeletePaperAsync([AliasAs("id")] Guid paperId);

    /// <summary>
    /// GET /papers/{id}/sections — returns all sections for a paper.
    /// </summary>
    [Get("/papers/{id}/sections")]
    Task<HttpResponseMessage> GetSectionsByPaperIdAsync([AliasAs("id")] Guid paperId);

    #endregion

    #region Paper Contributors

    /// <summary>
    /// POST /author/paper-contributors — creates a paper contributor record in Lab service.
    /// </summary>
    [Post("/author/paper-contributors")]
    Task<HttpResponseMessage> CreatePaperContributorAsync([Body] CreatePaperContributorRequest body);

    #endregion
}

public sealed class CreatePaperContributorRequest
{
    public string SectionRole { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }
}

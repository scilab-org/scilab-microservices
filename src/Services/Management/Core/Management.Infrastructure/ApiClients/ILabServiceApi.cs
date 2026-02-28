using Refit;

namespace Management.Infrastructure.ApiClients;

public interface ILabServiceApi
{
    #region Papers

    /// <summary>
    /// GET /papers — returns all papers (paged).
    /// </summary>
    [Get("/papers")]
    Task<HttpResponseMessage> GetPapersAsync(
        [AliasAs("pageNumber")] int pageNumber = 1,
        [AliasAs("pageSize")] int pageSize = 1000,
        [AliasAs("title")] string? title = null);

    /// <summary>
    /// GET /papers/{paperId} — returns a single paper by id.
    /// </summary>
    [Get("/papers/{paperId}")]
    Task<HttpResponseMessage> GetPaperByIdAsync([AliasAs("paperId")] Guid paperId);

    #endregion
}


using Refit;

namespace Lab.Infrastructure.ApiClients;

public interface IAiServiceApi
{
    /// <summary>
    /// POST /papers/format-style — reformats a paper's LaTeX content to match
    /// the conference/journal template style using an AI agent.
    /// </summary>
    [Post("/papers/format-style")]
    Task<HttpResponseMessage> FormatPaperStyleAsync([Body] FormatPaperStyleApiRequest body, CancellationToken cancellationToken = default);
}

public sealed class FormatPaperStyleApiRequest
{
    public string PaperContent { get; set; } = null!;
    public string TemplateContent { get; set; } = null!;
}

namespace Lab.Application.Services;

public interface IAiApiService
{
    /// <summary>
    /// Calls the AI service to reformat a paper's LaTeX content to match
    /// the conference/journal template style.
    /// The AI agent preserves all written content and only changes structural
    /// LaTeX formatting (document class, packages, author blocks, section commands,
    /// bibliography style, etc.).
    /// </summary>
    /// <param name="paperContent">The full LaTeX content of the paper.</param>
    /// <param name="templateContent">The conference/journal LaTeX template content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reformatted LaTeX content string.</returns>
    Task<string> FormatPaperToStyleAsync(
        string paperContent,
        string templateContent,
        CancellationToken cancellationToken = default);
}

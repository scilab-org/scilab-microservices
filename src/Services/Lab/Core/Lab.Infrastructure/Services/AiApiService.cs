using System.Net.Http.Json;
using Lab.Application.Services;
using Lab.Infrastructure.ApiClients;

namespace Lab.Infrastructure.Services;

public sealed class AiApiService(IAiServiceApi aiServiceApi) : IAiApiService
{
    public async Task<string> FormatPaperToStyleAsync(
        string paperContent,
        string templateContent,
        CancellationToken cancellationToken = default)
    {
        var response = await aiServiceApi.FormatPaperStyleAsync(
            new FormatPaperStyleApiRequest
            {
                PaperContent = paperContent,
                TemplateContent = templateContent,
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"AI service returned {(int)response.StatusCode}: {errorBody}");
        }

        if (response.Content is null)
        {
            throw new InvalidOperationException("AI service returned no response content.");
        }

        if (response.Content.Headers.ContentLength == 0)
        {
            throw new InvalidOperationException("AI service returned an empty response body.");
        }

        var body = await response.Content
            .ReadFromJsonAsync<FormatPaperStyleApiResult>(cancellationToken: cancellationToken);

        if (body is null)
        {
            throw new InvalidOperationException("AI service returned no JSON payload.");
        }

        return body.FormattedContent
               ?? throw new InvalidOperationException("AI service response did not contain formatted content.");
    }
}

/// <summary>Internal DTO matching the AI service JSON response shape (camelCase).</summary>
file sealed class FormatPaperStyleApiResult
{
    public string? FormattedContent { get; init; }
}

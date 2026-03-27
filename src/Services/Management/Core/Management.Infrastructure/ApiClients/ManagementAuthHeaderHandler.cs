using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace Management.Infrastructure.ApiClients;

public sealed class ManagementAuthHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(token) && AuthenticationHeaderValue.TryParse(token, out var headerValue))
            request.Headers.Authorization = headerValue;

        return base.SendAsync(request, cancellationToken);
    }
}

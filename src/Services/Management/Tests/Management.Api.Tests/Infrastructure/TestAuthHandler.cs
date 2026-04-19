using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Management.Api.Tests.Infrastructure;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public new const string Scheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>();

        if (Request.Headers.TryGetValue("X-Test-UserId", out var userId))
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId!));

        if (Request.Headers.TryGetValue("X-Test-Email", out var email))
            claims.Add(new Claim(ClaimTypes.Email, email!));

        if (Request.Headers.TryGetValue("X-Test-UserName", out var userName))
            claims.Add(new Claim("preferred_username", userName!));

        if (Request.Headers.TryGetValue("X-Test-FirstName", out var firstName))
            claims.Add(new Claim(ClaimTypes.GivenName, firstName!));

        if (Request.Headers.TryGetValue("X-Test-LastName", out var lastName))
            claims.Add(new Claim(ClaimTypes.Surname, lastName!));

        if (Request.Headers.TryGetValue("X-Test-Groups", out var groupValues))
        {
            foreach (var group in groupValues)
                if (!string.IsNullOrWhiteSpace(group))
                    claims.Add(new Claim("groups", group));
        }

        var identity = new ClaimsIdentity(claims, Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

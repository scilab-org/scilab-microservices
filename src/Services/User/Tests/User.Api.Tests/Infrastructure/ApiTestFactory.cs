using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using User.Api.Endpoints;

namespace User.Api.Tests.Infrastructure;

public sealed class ApiTestFactory : WebApplicationFactory<ApiTestFactory>
{
    public Mock<ISender> SenderMock { get; } = new();

    protected override IHost CreateHost(IHostBuilder hostBuilder)
    {
        // Touch User.Api assembly so Carter discovers its modules
        _ = typeof(CreateUser).Assembly;

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddRouting();
        builder.Services.AddAuthentication(TestAuthHandler.Scheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
        builder.Services.AddAuthorization(opts =>
            opts.AddPolicy("admin", p => p.RequireAuthenticatedUser()));
        builder.Services.AddCarter();
        builder.Services.AddSingleton(SenderMock.Object);
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Catch UnauthorizedAccessException (thrown by endpoints as authorization guard)
        // and convert it to 403 so tests can assert on the HTTP response
        app.Use(async (context, next) =>
        {
            try { await next(context); }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();

        app.Start();
        return app;
    }

    /// <summary>
    /// Creates an HttpClient authenticated with the given group claims.
    /// Pass no groups to create a client with no group claims.
    /// </summary>
    public HttpClient CreateTestClient(params string[] groups)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", "test-user-id");
        client.DefaultRequestHeaders.Add("X-Test-Email", "test@example.com");
        client.DefaultRequestHeaders.Add("X-Test-UserName", "testuser");
        client.DefaultRequestHeaders.Add("X-Test-FirstName", "Test");
        client.DefaultRequestHeaders.Add("X-Test-LastName", "User");
        foreach (var group in groups)
            client.DefaultRequestHeaders.Add("X-Test-Groups", group);
        return client;
    }
}

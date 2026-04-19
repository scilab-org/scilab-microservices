using AutoMapper;
using Carter;
using Management.Api.Mapping;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Management.Api.Endpoints;

namespace Management.Api.Tests.Infrastructure;

public sealed class ApiTestFactory : WebApplicationFactory<ApiTestFactory>
{
    public Mock<ISender> SenderMock { get; } = new();

    protected override IHost CreateHost(IHostBuilder hostBuilder)
    {
        // Touch Management.Api assembly so Carter discovers its modules
        _ = typeof(CreateProject).Assembly;

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();

        builder.Services.AddRouting();
        builder.Services.AddAuthentication(TestAuthHandler.Scheme)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
        builder.Services.AddAuthorization();
        builder.Services.AddCarter();
        builder.Services.AddSingleton(SenderMock.Object);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAutoMapper(typeof(ManagementApiMappingProfile));

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            try { await next(context); }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            catch (BuildingBlocks.Exceptions.NoPermissionException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            catch (BuildingBlocks.Exceptions.UnauthorizedException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            catch (BuildingBlocks.Exceptions.ClientValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapCarter();

        app.Start();
        return app;
    }

    public HttpClient CreateTestClient(params string[] groups)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Add("X-Test-Email", "test@example.com");
        client.DefaultRequestHeaders.Add("X-Test-UserName", "testuser");
        client.DefaultRequestHeaders.Add("X-Test-FirstName", "Test");
        client.DefaultRequestHeaders.Add("X-Test-LastName", "User");
        foreach (var group in groups)
            client.DefaultRequestHeaders.Add("X-Test-Groups", group);
        return client;
    }

    public HttpClient CreateUnauthenticatedClient()
    {
        return CreateClient();
    }
}

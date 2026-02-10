#region using

using System.Reflection;
using BuildingBlocks.Authentication.Extensions;
using BuildingBlocks.Swagger.Extensions;
using Common.Configurations;
using Common.Constants;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

#endregion

namespace Management.Api;

public static class DependencyInjection
{
    #region Methods

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        // services.AddDistributedTracing(cfg);
        // services.AddSerilogLogging(cfg);
        services.AddCarter();
        // CORS Configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        // HealthChecks
        {
            var dbType = cfg[$"{ConnectionStringsCfg.Section}:{ConnectionStringsCfg.DbType}"];
            var conn = cfg[$"{ConnectionStringsCfg.Section}:{ConnectionStringsCfg.Database}"];

            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    services.AddHealthChecks()
                        .AddSqlServer(connectionString: conn!);
                    break;
                case DatabaseType.MySql:
                    services.AddHealthChecks()
                        .AddMySql(connectionString: conn!);
                    break;
                case DatabaseType.PostgreSql:
                    services.AddHealthChecks()
                        .AddNpgSql(connectionString: conn!);
                    break;
                default:
                    throw new Exception("Unsupported database type");
            }
        }

        services.AddHttpContextAccessor();
        services.AddAuthenticationAndAuthorization(cfg);
        services.AddSwaggerServices(cfg);

        // Register all AutoMapper profiles from the current assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }

    public static WebApplication UseApi(this WebApplication app)
    {
        // app.UseSerilogReqLogging();
        // app.UsePrometheusEndpoint();
        app.MapCarter();
        app.UseExceptionHandler(options => { });
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwaggerApi();

        app.MapGet("/", (IWebHostEnvironment env) => new ApiDefaultPathResponse
        {
            Service = "Management.Api",
            Status = "Running",
            Timestamp = DateTimeOffset.UtcNow,
            Environment = env.EnvironmentName,
            Endpoints = new Dictionary<string, string>
            {
                { "health", "/health" }
            },
            Message = "API is running..."
        });

        return app;
    }

    #endregion
}
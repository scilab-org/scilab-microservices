#region using

using Management.Domain.Entities;
using Management.Infrastructure.ApiClients;
using Management.Infrastructure.ApiClients.Extensions;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Refit;

#endregion

namespace Management.Infrastructure;

public static class DependencyInjection
{
    #region Methods

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddMarten(opts =>
        {
            opts.Connection(cfg[$"{ConnectionStringsCfg.Section}:{ConnectionStringsCfg.Database}"]!);
            opts.UseSystemTextJsonForSerialization();
            
            opts.Schema.For<ProjectEntity>().SoftDeleted();
            opts.Schema.For<MemberEntity>().SoftDeleted();
        }).UseLightweightSessions();

        services.Scan(s => s
            .FromAssemblyOf<InfrastructureMarker>()
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Service")))
            .UsingRegistrationStrategy(Scrutor.RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s
            .FromAssemblyOf<InfrastructureMarker>()
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
            .UsingRegistrationStrategy(Scrutor.RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(cfg[$"{MinIoCfg.Section}:{MinIoCfg.Endpoint}"])
            .WithCredentials(cfg[$"{MinIoCfg.Section}:{MinIoCfg.AccessKey}"],
                cfg[$"{MinIoCfg.Section}:{MinIoCfg.SecretKey}"])
            .WithSSL(cfg.GetValue<bool>($"{MinIoCfg.Section}:{MinIoCfg.Secure}"))
            .Build());

        //services.InitializeMartenWith<InitialData>();

        services.AddRefitClient<IUserServiceApi>()
            .AddHttpMessageHandler<ManagementAuthHeaderHandler>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.UserService.Section}:{ApiClientCfg.UserService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });
        services.AddRefitClient<ILabServiceApi>()
            .AddHttpMessageHandler<ManagementAuthHeaderHandler>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.LabService.Section}:{ApiClientCfg.LabService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });
        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        return app;
    }

    #endregion
}
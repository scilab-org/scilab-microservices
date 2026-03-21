#region using

using Lab.Domain.Entities;
using Lab.Infrastructure.ApiClients;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Refit;

#endregion

namespace Lab.Infrastructure;

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

            opts.Schema.For<PaperEntity>()
                .SoftDeleted()
                .Index(p => p.TagNames);
            opts.Schema.For<PaperBankEntity>()
                .SoftDeleted()
                .Index(pb => pb.TagNames);
            opts.Schema.For<TagEntity>()
                .SoftDeleted()
                .Index(t => t.Name, idx => { idx.IsUnique = true; });
            opts.Schema.For<JournalEntity>()
                .SoftDeleted();
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
                    .WithCredentials(cfg[$"{MinIoCfg.Section}:{MinIoCfg.AccessKey}"], cfg[$"{MinIoCfg.Section}:{MinIoCfg.SecretKey}"])
                    .WithSSL(cfg.GetValue<bool>($"{MinIoCfg.Section}:{MinIoCfg.Secure}"))
                    .Build());

        services.AddTransient<ManagementAuthHeaderHandler>();

        services.AddRefitClient<IManagementServiceApi>()
            .AddHttpMessageHandler<ManagementAuthHeaderHandler>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.ManagementService.Section}:{ApiClientCfg.ManagementService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddRefitClient<IUserServiceApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.UserService.Section}:{ApiClientCfg.UserService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        //services.InitializeMartenWith<InitialData>();

        return services;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        return app;
    }

    #endregion
}

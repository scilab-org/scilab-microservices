#region using

using Lab.Domain.Entities;
using Lab.Infrastructure.ApiClients;
using Marten;
using Microsoft.AspNetCore.Builder;
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
        IConfiguration cfg,
        bool useHttpAuth = true)
    {
        services.AddMarten(opts =>
        {
            opts.Connection(cfg[$"{ConnectionStringsCfg.Section}:{ConnectionStringsCfg.Database}"]!);
            opts.UseSystemTextJsonForSerialization();

            opts.Schema.For<PaperEntity>()
                .SoftDeleted();
            opts.Schema.For<SectionEntity>()
                .SoftDeleted();
            opts.Schema.For<PaperBankEntity>()
                .SoftDeleted()
                .Index(pb => pb.TagNames);
            opts.Schema.For<TagEntity>()
                .SoftDeleted()
                .Index(t => t.Name, idx => { idx.IsUnique = true; });
            opts.Schema.For<ConferenceJournalEntity>()
                .SoftDeleted();
            opts.Schema.For<PaperContributorEntity>()
                .SoftDeleted();
            opts.Schema.For<PaperStatusHistoryEntity>()
                .Index(h => h.PaperId);
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

        var managementClientBuilder = services.AddRefitClient<IManagementServiceApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.ManagementService.Section}:{ApiClientCfg.ManagementService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        if (useHttpAuth)
        {
            services.AddTransient<ManagementAuthHeaderHandler>();
            managementClientBuilder.AddHttpMessageHandler<ManagementAuthHeaderHandler>();
        }

        services.AddRefitClient<IUserServiceApi>()
            .AddHttpMessageHandler<ManagementAuthHeaderHandler>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.UserService.Section}:{ApiClientCfg.UserService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddRefitClient<IAiServiceApi>()
            .AddHttpMessageHandler<ManagementAuthHeaderHandler>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.AiService.Section}:{ApiClientCfg.AiService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromMinutes(5);
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
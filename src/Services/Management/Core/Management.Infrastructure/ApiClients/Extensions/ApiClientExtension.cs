using Management.Infrastructure.ApiClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Management.Infrastructure.ApiClients.Extensions;

public static class ApiClientExtension
{
    #region Methods

    public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddRefitClient<IUserServiceApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.UserService.Section}:{ApiClientCfg.UserService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        services.AddRefitClient<ILabServiceApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.LabService.Section}:{ApiClientCfg.LabService.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });

        return services;
    }

    #endregion
}


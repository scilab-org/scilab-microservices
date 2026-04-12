#region using

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

#endregion

using System.Diagnostics.CodeAnalysis;

namespace User.Infrastructure.ApiClients.Extensions;

[ExcludeFromCodeCoverage]
public static class ApiClientExtension
{
    #region Methods

    public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddRefitClient<IKeycloakApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(cfg[$"{ApiClientCfg.Keycloak.Section}:{ApiClientCfg.Keycloak.BaseUrl}"]!);
                c.Timeout = TimeSpan.FromSeconds(30);
            });
        
        return services;
    }

    #endregion

}
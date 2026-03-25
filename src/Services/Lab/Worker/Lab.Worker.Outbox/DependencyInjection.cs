using Lab.Worker.Outbox.Processors;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EventSourcing.MassTransit;
using BuildingBlocks.Logging;

namespace Lab.Worker.Outbox;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerServices(
      this IServiceCollection services,
      IConfiguration cfg)
    {
        //services.AddSerilogLogging(cfg);
        services.AddMessageBroker(cfg, Assembly.GetExecutingAssembly());
        services.AddScoped<OutboxProcessor>();

        return services;
    }
}

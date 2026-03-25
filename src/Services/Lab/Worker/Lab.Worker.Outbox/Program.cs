using Lab.Application;
using Lab.Infrastructure;
using Lab.Worker.Outbox;
using Lab.Worker.Outbox.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration, useHttpAuth: false)
    .AddWorkerServices(builder.Configuration)   
    .AddHostedService<OutboxBackgroundService>();

var host = builder.Build();
host.Run();

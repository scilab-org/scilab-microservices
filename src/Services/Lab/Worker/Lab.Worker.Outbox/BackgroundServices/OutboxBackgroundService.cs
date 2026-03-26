using System;
using System.Collections.Generic;
using System.Text;
using Common.Configurations;
using Lab.Worker.Outbox.Processors;

namespace Lab.Worker.Outbox.BackgroundServices;

internal class OutboxBackgroundService : BackgroundService
{
    private readonly int _processorFrequency;

    private int _totalIterations = 0;

    private int _totalProcessedMessage = 0;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ILogger<OutboxBackgroundService> _logger;

    public OutboxBackgroundService(
       IServiceScopeFactory serviceScopeFactory,
       IConfiguration cfg,
       ILogger<OutboxBackgroundService> logger)
    {
        _processorFrequency = cfg.GetValue<int>($"{WorkerCfg.Outbox.Section}:{WorkerCfg.Outbox.ProcessorFrequency}", 5);
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor started (single-worker mode)");

        // Run a SINGLE polling loop.  Publishing concurrency is handled
        // inside OutboxProcessor.ExecuteAsync (Task.WhenAll over the batch).
        // Running multiple parallel loops caused duplicate claims because
        // each loop had its own Marten session with no cross-visibility.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

                int processedMessages = await outboxProcessor.ExecuteAsync(stoppingToken);
                _totalProcessedMessage += processedMessages;

                var iterationCount = ++_totalIterations;
                if (processedMessages > 0 || iterationCount % 100 == 0)
                {
                    _logger.LogInformation(
                        "Iteration {IterationCount}: Processed {ProcessedMessages} messages. Total: {TotalProcessedMessages}",
                        iterationCount, processedMessages, _totalProcessedMessage);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during outbox processing iteration");
            }

            await Task.Delay(TimeSpan.FromSeconds(_processorFrequency), stoppingToken);
        }

        _logger.LogInformation("Outbox processor stopped");
    }
}


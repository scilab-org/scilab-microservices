using EventSourcing.Events.Lab;
using MassTransit;

namespace Lab.Api.Consumers;

public class PaperIngestionConsumer(ILogger<PaperIngestionConsumer> logger) : IConsumer<PaperIngestionEvent>
{
    public Task Consume(ConsumeContext<PaperIngestionEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "[PaperIngestionConsumer] Received PaperIngestionEvent — " +
            "Id: {EventId}, PaperId: {PaperId}, PaperName: {PaperName}, OccurredOn: {OccurredOn}",
            msg.Id,
            msg.PaperId,
            msg.PaperName,
            msg.OccurredOn);

        logger.LogDebug(
            "[PaperIngestionConsumer] ParsedText for PaperId {PaperId}: {ParsedText}",
            msg.PaperId,
            msg.ParsedText);

        return Task.CompletedTask;
    }
}

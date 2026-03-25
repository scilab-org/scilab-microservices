using EventSourcing.Events.Lab;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBank;
using Lab.Domain.Enums;
using MassTransit;

namespace Lab.Api.Consumers;


public class PaperIngestionCompletedConsumer(IMediator mediator, ILogger<PaperIngestionCompletedConsumer> logger) : IConsumer<PaperIngestionCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaperIngestionCompletedEvent> context)
    {
        logger.LogInformation("Received PaperIngestionCompletedEvent for PaperId: {PaperId}, IsSuccess: {IsSuccess}", context.Message.PaperId, context.Message.IsSuccess);
        var msg = context.Message;

        await mediator.Send(new UpdatePaperBankCommand(
            msg.PaperId,
            new UpdatePaperBankDto
            {
                IsIngested = msg.IsSuccess,
                IngestStatus = msg.IsSuccess ? IngestStatus.Success : IngestStatus.Failed,
            }
        ));
    }
}

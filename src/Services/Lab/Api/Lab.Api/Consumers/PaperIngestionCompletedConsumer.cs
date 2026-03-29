using EventSourcing.Events.Lab;
using Lab.Application.Features.PaperBank.Commands.UpdatePaperBankIngestionStatus;
using MassTransit;
using IMediator = MediatR.IMediator;

namespace Lab.Api.Consumers;

public class PaperIngestionCompletedConsumer(IMediator mediator, ILogger<PaperIngestionCompletedConsumer> logger) : IConsumer<PaperIngestionCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaperIngestionCompletedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Received PaperIngestionCompletedEvent for PaperId: {PaperId}, IsSuccess: {IsSuccess}",
            msg.PaperId, msg.IsSuccess);

        await mediator.Send(new UpdatePaperBankIngestionStatusCommand(
            msg.PaperId,
            msg.IsSuccess,
            msg.ErrorMessage));
    }
}

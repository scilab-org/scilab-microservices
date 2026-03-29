using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Commands.UpdatePaperBankIngestionStatus;

public record UpdatePaperBankIngestionStatusCommand(
    Guid PaperId,
    bool IsSuccess,
    string? ErrorMessage) : ICommand<Guid>;

public class UpdatePaperBankIngestionStatusValidator : AbstractValidator<UpdatePaperBankIngestionStatusCommand>
{
    public UpdatePaperBankIngestionStatusValidator()
    {
        RuleFor(x => x.PaperId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class UpdatePaperBankIngestionStatusHandler(IDocumentSession session)
    : IRequestHandler<UpdatePaperBankIngestionStatusCommand, Guid>
{
    public async Task<Guid> Handle(UpdatePaperBankIngestionStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<PaperBankEntity>(request.PaperId, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.PaperId);

        entity.UpdateIngestionStatus(
            isIngested: request.IsSuccess,
            ingestStatus: request.IsSuccess ? IngestStatus.Success : IngestStatus.Failed);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

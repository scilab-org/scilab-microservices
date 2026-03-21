using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.DeleteJournal;

public record DeleteJournalCommand(Guid Id) : ICommand<Unit>;

public class DeleteJournalCommandValidator : AbstractValidator<DeleteJournalCommand>
{
    public DeleteJournalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);
    }
}

public class DeleteJournalCommandHandler(IDocumentSession session) : IRequestHandler<DeleteJournalCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteJournalCommand request, CancellationToken cancellationToken)
    {
        var journal = await session.LoadAsync<JournalEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id.ToString());

        session.Delete(journal);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
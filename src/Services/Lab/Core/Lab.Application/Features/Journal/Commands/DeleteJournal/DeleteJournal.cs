using JasperFx.Core;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.DeleteJournal;

public record DeleteJournalCommand(Guid Id, string UserName) : ICommand<Unit>;

public class DeleteJournalCommandValidator : AbstractValidator<DeleteJournalCommand>
{
    public DeleteJournalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);

    }
}

public class DeleteJournalCommandHandler(IDocumentSession session, IManagementApiService managementApiService)
    : IRequestHandler<DeleteJournalCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteJournalCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);
        var journal = await session.LoadAsync<ConferenceJournalEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id.ToString());

        await managementApiService.RemoveConferenceJournalFromProjectAsync(
            journal.Id,
            cancellationToken);

        journal.Update(lastModifiedBy: request.UserName);

        session.Update(journal);
        session.Delete(journal);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
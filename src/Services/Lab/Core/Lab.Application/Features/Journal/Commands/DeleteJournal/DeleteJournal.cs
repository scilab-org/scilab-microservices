using JasperFx.Core;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Journal.Commands.DeleteJournal;

public record DeleteJournalCommand(Guid Id, Guid ProjectId, string UserName) : ICommand<Unit>;

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
        var journal = await session.Query<ConferenceJournalEntity>()
                          .FirstOrDefaultAsync(x => x.Id == request.Id,
                              cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id.ToString());

        var role = await managementApiService.GetMyProjectRoleAsync(journal.ProjectId, cancellationToken);
        if (string.IsNullOrEmpty(role) && !AuthorizeConstants.ProjectManager.EqualsIgnoreCase(role!))
        {
            throw new UnauthorizedException(MessageCode.Unauthorized);
        }

        var synced = await managementApiService.RemoveProjectConferenceJournalsAsync(
            journal.ProjectId,
            [journal.Id],
            cancellationToken);
        if (!synced)
            throw new ClientValidationException(MessageCode.ProjectIsNotExists, journal.ProjectId.ToString());

        journal.Update(lastModifiedBy: request.UserName);

        session.Update(journal);
        session.Delete(journal);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
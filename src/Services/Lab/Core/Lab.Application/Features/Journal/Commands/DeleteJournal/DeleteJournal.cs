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

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.JournalProjectIdIsRequired);
    }
}

public class DeleteJournalCommandHandler(IDocumentSession session, IManagementApiService managementApiService)
    : IRequestHandler<DeleteJournalCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteJournalCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.ProjectId, cancellationToken);
        if (string.IsNullOrEmpty(role) && !AuthorizeConstants.ProjectManager.EqualsIgnoreCase(role!))
        {
            throw new UnauthorizedException(MessageCode.Unauthorized);
        }

        var journal = await session.Query<ConferenceJournalEntity>()
                          .FirstOrDefaultAsync(x => x.Id == request.Id && x.ProjectId == request.ProjectId,
                              cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.JournalIsNotExists, request.Id.ToString());

        journal.Update(lastModifiedBy: request.UserName);

        session.Update(journal);
        session.Delete(journal);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteProjectConferenceJournalsCommand(Guid ProjectId, Guid JournalId)
    : ICommand<Guid>;

public class DeleteProjectConferenceJournalsValidator : AbstractValidator<DeleteProjectConferenceJournalsCommand>
{
    public DeleteProjectConferenceJournalsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.JournalId)
            .NotEmpty()
            .WithMessage(MessageCode.ConferenceJournalIdIsRequired);
    }
}

public class DeleteProjectConferenceJournalsCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteProjectConferenceJournalsCommand, Guid>
{
    public async Task<Guid> Handle(DeleteProjectConferenceJournalsCommand command,
        CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        project.ConferenceJournalIds.Remove(command.JournalId);

        session.Update(project);
        await session.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteProjectConferenceJournalByJournalIdCommand(Guid JournalId) : ICommand<List<Guid>>;

public class DeleteProjectConferenceJournalByJournalIdCommandValidator : AbstractValidator<DeleteProjectConferenceJournalByJournalIdCommand>
{
    public DeleteProjectConferenceJournalByJournalIdCommandValidator()
    {
        RuleFor(x => x.JournalId)
            .NotEmpty()
            .WithMessage(MessageCode.JournalIdIsRequired);
    }
}

public class DeleteProjectConferenceJournalByJournalIdCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteProjectConferenceJournalByJournalIdCommand, List<Guid>>
{
    public async Task<List<Guid>> Handle(DeleteProjectConferenceJournalByJournalIdCommand command, CancellationToken cancellationToken)
    {
        var projects = await session.Query<ProjectEntity>()
            .Where(x => x.ConferenceJournalIds.Contains(command.JournalId))
            .ToListAsync(cancellationToken);

        if (!projects.Any())
            return [];

        var journalIdSet = new HashSet<Guid> { command.JournalId };
        var affectedProjectIds = new List<Guid>();

        foreach (var project in projects)
        {
            var removedJournalIds = project.RemoveConferenceJournals(journalIdSet);
            if (removedJournalIds.Count == 0)
                continue;

            affectedProjectIds.Add(project.Id);
            session.Store(project);
        }

        if (!affectedProjectIds.Any())
            return affectedProjectIds;

        await session.SaveChangesAsync(cancellationToken);

        return affectedProjectIds;
    }
}
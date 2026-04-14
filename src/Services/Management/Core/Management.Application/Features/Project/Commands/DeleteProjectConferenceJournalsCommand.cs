using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteProjectConferenceJournalsCommand(Guid ProjectId, DeleteProjectConferenceJournalDto Dto)
    : ICommand<List<Guid>>;

public class DeleteProjectConferenceJournalsValidator : AbstractValidator<DeleteProjectConferenceJournalsCommand>
{
    public DeleteProjectConferenceJournalsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto.ConferenceJournalIds)
            .NotEmpty()
            .WithMessage(MessageCode.ConferenceJournalIdsAreRequired);
    }
}

public class DeleteProjectConferenceJournalsCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteProjectConferenceJournalsCommand, List<Guid>>
{
    public async Task<List<Guid>> Handle(DeleteProjectConferenceJournalsCommand command, CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        var ids = command.Dto.ConferenceJournalIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (!ids.Any())
            throw new ClientValidationException(MessageCode.ConferenceJournalIdsAreRequired);

        var removed = project.RemoveConferenceJournals(ids);

        if (!removed.Any())
            throw new NotFoundException(MessageCode.ConferenceJournalNotFoundInProject);

        session.Store(project);
        await session.SaveChangesAsync(cancellationToken);

        return removed;
    }
}
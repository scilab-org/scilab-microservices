using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record CreateProjectConferenceJournalCommand(Guid ProjectId, Guid JournalId)
    : ICommand<Guid>;

public class CreateProjectConferenceJournalValidator : AbstractValidator<CreateProjectConferenceJournalCommand>
{
    public CreateProjectConferenceJournalValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.JournalId)
            .NotEmpty()
            .WithMessage(MessageCode.ConferenceJournalIdIsRequired);
    }
}

public class CreateProjectConferenceJournalCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProjectConferenceJournalCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectConferenceJournalCommand command,
        CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        var list = project.ConferenceJournalIds;

        list.Add(command.JournalId);
        list = list.Distinct().ToList();

        project.Update(conferenceJournalIds: list);

        session.Update(project);
        await session.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
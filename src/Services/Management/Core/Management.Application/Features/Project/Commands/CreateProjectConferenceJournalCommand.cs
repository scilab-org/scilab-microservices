using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record CreateProjectConferenceJournalCommand(Guid ProjectId, CreateProjectConferenceJournalDto Dto)
    : ICommand<List<Guid>>;

public class CreateProjectConferenceJournalValidator : AbstractValidator<CreateProjectConferenceJournalCommand>
{
    public CreateProjectConferenceJournalValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto.ConferenceJournalIds)
            .NotEmpty()
            .WithMessage(MessageCode.ConferenceJournalIdsAreRequired);
    }
}

public class CreateProjectConferenceJournalCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProjectConferenceJournalCommand, List<Guid>>
{
    public async Task<List<Guid>> Handle(CreateProjectConferenceJournalCommand command, CancellationToken cancellationToken)
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

        project.AddConferenceJournals(ids);

        session.Store(project);
        await session.SaveChangesAsync(cancellationToken);

        return ids;
    }
}
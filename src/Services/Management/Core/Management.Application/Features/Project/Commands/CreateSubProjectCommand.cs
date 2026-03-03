using Management.Application.Dtos.Projects;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record CreateSubProjectCommand(Guid ProjectId, CreateSubProjectDto Dto) : ICommand<Guid>;

public class CreateSubProjectValidator : AbstractValidator<CreateSubProjectCommand>
{
    public CreateSubProjectValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto.PaperId)
            .NotEmpty()
            .WithMessage("PAPER_IDS_REQUIRED");
    }
}

public class CreateSubProjectCommandHandler(
    IDocumentSession session) : ICommandHandler<CreateSubProjectCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateSubProjectCommand command, CancellationToken cancellationToken)
    {
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if(project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        var subProject = ProjectEntity.Create(
            id: Guid.NewGuid(),
            parentProjectId: command.ProjectId,
            name: command.Dto.Name);

        subProject.AddPapers(new List<Guid> { command.Dto.PaperId });

        session.Store(subProject);
        await session.SaveChangesAsync(cancellationToken);

        return subProject.Id;
    }
    #endregion
}
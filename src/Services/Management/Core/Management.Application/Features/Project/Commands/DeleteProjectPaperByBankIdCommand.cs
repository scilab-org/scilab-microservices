using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteProjectPaperByBankIdCommand(Guid PaperBankId)  : ICommand<List<Guid>>;

public class DeleteProjectPaperByBankIdCommandValidator : AbstractValidator<DeleteProjectPaperByBankIdCommand>
{
    public DeleteProjectPaperByBankIdCommandValidator()
    {
        RuleFor(x => x.PaperBankId)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

    }
}

public class DeleteProjectPaperByBankIdCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteProjectPaperByBankIdCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(DeleteProjectPaperByBankIdCommand command, CancellationToken cancellationToken)
    {
        var projects = await session.Query<ProjectEntity>()
            .Where(x => x.PaperIds.Contains(command.PaperBankId))
            .ToListAsync(cancellationToken);
        
        if (!projects.Any())
            throw new NotFoundException(MessageCode.PaperNotFoundInProject);

        var paperIdSet = new HashSet<Guid> { command.PaperBankId };

        var removedPaperIds = new List<Guid>();
        
        foreach (var project in projects)
        {
            removedPaperIds.AddRange(project.RemovePapers(paperIdSet));
            session.Store(project);
        }
        
        if (removedPaperIds.Count == 0)
            throw new NotFoundException(MessageCode.PaperNotFoundInProject);
        
        
        await session.SaveChangesAsync(cancellationToken);

        return removedPaperIds;
    }

    #endregion
}

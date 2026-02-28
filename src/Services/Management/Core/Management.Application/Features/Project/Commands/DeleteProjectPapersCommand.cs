using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteProjectPapersCommand(Guid ProjectId, DeleteProjectPaperDto Dto) : ICommand<List<Guid>>;

public class DeleteProjectPapersValidator : AbstractValidator<DeleteProjectPapersCommand>
{
    public DeleteProjectPapersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.PaperIds)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdsAreRequired);
    }
}

public class DeleteProjectPapersCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteProjectPapersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(DeleteProjectPapersCommand command, CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        var paperIdSet = command.Dto.PaperIds.Where(x => x != Guid.Empty).Distinct().ToHashSet();

        if (paperIdSet.Count == 0)
            throw new ClientValidationException(MessageCode.MemberIdsAreRequired);

        var papersToRemove = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == command.ProjectId && x.PaperId.HasValue && paperIdSet.Contains(x.PaperId.Value))
            .ToListAsync(cancellationToken);

        if (!papersToRemove.Any())
            throw new NotFoundException(MessageCode.MembersNotFound);

        var removedIds = new List<Guid>();
        foreach (var paper in papersToRemove)
        {
            session.Delete(paper);
            removedIds.Add(paper.Id);
        }

        await session.SaveChangesAsync(cancellationToken);

        return removedIds;
    }

    #endregion
}
using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public record AddPaperProjectCommand(Guid ParentProjectId, AddPaperProjectDto Dto) : ICommand<List<Guid>>;

public class AddPaperProjectValidator : AbstractValidator<AddPaperProjectCommand>
{
    public AddPaperProjectValidator()
    {
        RuleFor(x => x.ParentProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto.PaperIds)
            .NotEmpty()
            .WithMessage("PAPER_IDS_REQUIRED");
    }
}

public class AddPaperProjectCommandHandler(IDocumentSession session) : ICommandHandler<AddPaperProjectCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(AddPaperProjectCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var parentProject = await session.LoadAsync<ProjectEntity>(command.ParentProjectId, cancellationToken);
        if(parentProject == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == command.ParentProjectId)
            .ToListAsync(cancellationToken);
        
        var createdIds = new List<Guid>();
        await session.BeginTransactionAsync(cancellationToken);
        
        foreach (var paperId in dto.PaperIds.Distinct())
        {
            var alreadyExists = subProjects.Any(x => x.PaperId == paperId);
            if (alreadyExists) continue;
            
            var subId  = Guid.NewGuid();
            var subProject = ProjectEntity.Create(
                id: subId ,
                parentProjectId: command.ParentProjectId,
                paperId: paperId);
            session.Store(subProject);
            createdIds.Add(subId);
        }
        if (!createdIds.Any())
            throw new ClientValidationException(MessageCode.AllPapersAlreadyExist);

        await session.SaveChangesAsync(cancellationToken);

        return createdIds;
    }
    #endregion
}
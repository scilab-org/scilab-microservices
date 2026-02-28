using Management.Application.Dtos.Projects;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public record CreateProjectPaperCommand(Guid ParentProjectId, CreateProjectPaperDto Dto) : ICommand<List<Guid>>;

public class CreateProjectPaperValidator : AbstractValidator<CreateProjectPaperCommand>
{
    public CreateProjectPaperValidator()
    {
        RuleFor(x => x.ParentProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto.PaperIds)
            .NotEmpty()
            .WithMessage("PAPER_IDS_REQUIRED");
    }
}

public class CreateProjectPaperCommandHandler(
    IDocumentSession session,
    ILabApiService labApiService) : ICommandHandler<CreateProjectPaperCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(CreateProjectPaperCommand command, CancellationToken cancellationToken)
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
        
        var validPaperIds = await labApiService.GetExistingPaperIdsAsync(dto.PaperIds, cancellationToken);
        if (validPaperIds.Count == 0)
            throw new NotFoundException(MessageCode.PaperIsNotExists);
        
        foreach (var paperId in validPaperIds.Distinct())
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
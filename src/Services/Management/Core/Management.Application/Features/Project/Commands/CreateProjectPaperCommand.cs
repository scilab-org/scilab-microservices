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

        var project = await session.LoadAsync<ProjectEntity>(command.ParentProjectId, cancellationToken);
        if(project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        var validPaperIds = await labApiService.GetExistingPaperIdsAsync(dto.PaperIds, cancellationToken);
        if (validPaperIds.Count == 0)
            throw new NotFoundException(MessageCode.PaperIsNotExists);
        
        project.AddPapers(validPaperIds);
        
        session.Store(project);
        await session.SaveChangesAsync(cancellationToken);

        return project.PaperIds;
    }
    #endregion
}
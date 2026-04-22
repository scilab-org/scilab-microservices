using Management.Application.Dtos.Projects;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public record UpdateProjectCommand(Guid ProjectId, UpdateProjectDto Dto) : ICommand<Guid>;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    #region Ctors

    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.ProjectNameIsRequired);
                RuleFor(x => x.Dto)
                    .Must(dto =>
                    {
                        if (dto.StartDate == null || dto.EndDate == null) return true;

                        return dto.StartDate < dto.EndDate;
                    })
                    .WithMessage(MessageCode.StartDateMustBeBeforeEndDate);
            });
    }

    #endregion
}

[ExcludeFromCodeCoverage]
public class UpdateProjectCommandHandler(
    IDocumentSession session,
    ILabApiService labApiService) : ICommandHandler<UpdateProjectCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException(MessageCode.ProjectIsNotExists, command.ProjectId);
        
        var dto = command.Dto;
        var nextContext = dto.Context;
        var nextDomains = dto.DomainIds;
        var nextKeypoint = dto.Keypoint;
        var requiresRuleSync = !string.Equals(entity.Context, nextContext, StringComparison.Ordinal)
                               || entity.DomainIds.Except(nextDomains).Any()
                               || nextDomains.Except(entity.DomainIds).Any()
                               || !string.Equals(entity.Keypoint, nextKeypoint, StringComparison.Ordinal);
        
        entity.Update(
            name: dto.Name!,
            code: dto.Code,
            description: dto.Description,
            status: dto.Status,
            startDate: dto.StartDate,
            endDate: dto.EndDate,
            context: nextContext,
            domainIds: nextDomains,
            keypoint: dto.Keypoint);

        var subProject = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == entity.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (requiresRuleSync && subProject != null)
        {
            var paperIds = await session.Query<ProjectEntity>()
                .Where(x => x.ParentProjectId == entity.Id)
                .SelectMany(x => x.PaperIds)
                .Distinct()
                .ToListAsync(cancellationToken);
            
            await labApiService.UpdateProjectRulesAsync(
                paperIds,
                nextContext,
                string.Join(",", nextDomains),
                nextKeypoint,
                cancellationToken);

        }

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}

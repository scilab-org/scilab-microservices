using Management.Application.Dtos.Projects;
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

public class UpdateProjectCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProjectCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.ProjectIsNotExists, command.ProjectId);

        var dto = command.Dto;
        
        entity.Update(
            name: dto.Name!,
            code: dto.Code,
            description: dto.Description,
            status: dto.Status,
            startDate: dto.StartDate,
            endDate: dto.EndDate);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}
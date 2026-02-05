using Management.Application.Dtos.Projects;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Commands;

public record CreateProjectCommand(CreateProjectDto Dto) : ICommand<Guid>;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    #region Ctors
    public CreateProjectCommandValidator()
    {
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

public class CreateProjectCommandHandler(IDocumentSession session) : ICommandHandler<CreateProjectCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var entity = ProjectEntity.Create(
            id: Guid.NewGuid(),
            name: dto.Name!,
            description: dto.Description,
            code: dto.Code,
            startDate: dto.StartDate,
            endDate: dto.EndDate);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}
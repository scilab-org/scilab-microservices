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

                        return dto.StartDate.Value < dto.EndDate.Value;
                    })
                    .WithMessage(MessageCode.StartDateMustBeBeforeEndDate);
            });
    }

    #endregion
}

[ExcludeFromCodeCoverage]
public class CreateProjectCommandHandler(IDocumentSession session) : ICommandHandler<CreateProjectCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        if (!string.IsNullOrWhiteSpace(dto.Code))
        {
            var checkCode = await session.Query<ProjectEntity>()
                .AnyAsync(p => p.Code == dto.Code, cancellationToken);
            if (checkCode)
                throw new ClientValidationException(MessageCode.ProjectCodeAlreadyExists, dto.Code);
        }

        await session.BeginTransactionAsync(cancellationToken);

        var entity = ProjectEntity.Create(
            id: Guid.NewGuid(),
            name: dto.Name!,
            description: dto.Description,
            code: dto.Code,
            status: dto.Status,
            startDate: dto.StartDate,
            endDate: dto.EndDate,
            context: dto.Context,
            domainIds: dto.DomainIds,
            keypoint: dto.Keypoint);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}

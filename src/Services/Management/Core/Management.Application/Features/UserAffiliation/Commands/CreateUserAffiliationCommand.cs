using Management.Application.Dtos.UserAffiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Commands;

public sealed record CreateUserAffiliationCommand(CreateUserAffiliationDto Dto) : ICommand<Guid>;

public sealed class CreateUserAffiliationCommandValidator : AbstractValidator<CreateUserAffiliationCommand>
{
    public CreateUserAffiliationCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.UserId)
                    .NotEmpty()
                    .WithMessage(MessageCode.BadRequest);

                RuleFor(x => x.Dto.AffiliationId)
                    .NotEmpty()
                    .WithMessage(MessageCode.BadRequest);
            });
    }
}

public sealed class CreateUserAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateUserAffiliationCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserAffiliationCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var entity = UserAffiliationEntity.Create(
            id,
            command.Dto.UserId,
            command.Dto.AffiliationId,
            command.Dto.Department,
            command.Dto.Position,
            command.Dto.AffiliationStartYear,
            command.Dto.AffiliationEndYear);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

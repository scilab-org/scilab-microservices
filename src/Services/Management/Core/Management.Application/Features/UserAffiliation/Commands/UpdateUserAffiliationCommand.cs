using Management.Application.Dtos.UserAffiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Commands;

public sealed record UpdateUserAffiliationCommand(Guid Id, UpdateUserAffiliationDto Dto) : ICommand<Guid>;

[ExcludeFromCodeCoverage]
public sealed class UpdateUserAffiliationCommandValidator : AbstractValidator<UpdateUserAffiliationCommand>
{
    public UpdateUserAffiliationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.BadRequest);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class UpdateUserAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateUserAffiliationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateUserAffiliationCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<UserAffiliationEntity>(command.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.UserAffiliationIsNotExists, command.Id.ToString());

        entity.Update(
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

using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Commands;

public sealed record DeleteUserAffiliationCommand(Guid Id) : ICommand<Unit>;

[ExcludeFromCodeCoverage]
public sealed class DeleteUserAffiliationCommandValidator : AbstractValidator<DeleteUserAffiliationCommand>
{
    public DeleteUserAffiliationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class DeleteUserAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteUserAffiliationCommand>
{
    public async Task<Unit> Handle(DeleteUserAffiliationCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<UserAffiliationEntity>(command.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.UserAffiliationIsNotExists, command.Id.ToString());

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

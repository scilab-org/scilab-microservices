using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Affiliation.Commands;

public sealed record DeleteAffiliationCommand(Guid Id) : ICommand<Unit>;

public sealed class DeleteAffiliationCommandValidator : AbstractValidator<DeleteAffiliationCommand>
{
    public DeleteAffiliationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class DeleteAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteAffiliationCommand>
{
    public async Task<Unit> Handle(DeleteAffiliationCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<AffiliationEntity>(command.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.AffiliationIsNotExists, command.Id.ToString());

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

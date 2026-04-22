using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Domain.Commands;

public sealed record DeleteDomainCommand(Guid DomainId) : ICommand<Unit>;

public sealed class DeleteDomainCommandValidator : AbstractValidator<DeleteDomainCommand>
{
    #region Ctors

    public DeleteDomainCommandValidator()
    {
        RuleFor(x => x.DomainId)
            .NotEmpty()
            .WithMessage(MessageCode.DomainIdIsRequired);
    }

    #endregion
}

public sealed class DeleteDomainCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteDomainCommand, Unit>
{
    #region Implementations
    public async Task<Unit> Handle(DeleteDomainCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<DomainEntity>(command.DomainId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.DomainIsNotExists, command.DomainId.ToString());

        var projects = await session.Query<ProjectEntity>()
            .Where(x => x.DomainIds.Contains(command.DomainId))
            .ToListAsync(cancellationToken);

        foreach (var project in projects)
        {
            project.DomainIds.Remove(command.DomainId);
            project.LastModifiedOnUtc = DateTimeOffset.UtcNow;
            session.Store(project);
        }

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value; 
    }
    #endregion
}

using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.AuthorRole.Commands.DeleteAuthorRole;

public record DeleteAuthorRoleCommand(Guid Id) : ICommand<Guid>;

public class DeleteAuthorRoleCommandValidator : AbstractValidator<DeleteAuthorRoleCommand>
{
    public DeleteAuthorRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(MessageCode.RoleNotFound);
    }
}

public class DeleteAuthorRoleCommandHandler(IDocumentSession session) : IRequestHandler<DeleteAuthorRoleCommand, Guid>
{
    public async Task<Guid> Handle(DeleteAuthorRoleCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<AuthorRoleEntity>(request.Id, cancellationToken);
        if (entity == null)
            throw new ClientValidationException(MessageCode.RoleNotFound, request.Id);

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}

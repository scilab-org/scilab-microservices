using Lab.Application.Dtos.AuthorRoles;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.AuthorRole.Commands.UpdateAuthorRole;

public record UpdateAuthorRoleCommand(Guid Id, UpdateAuthorRoleDto Dto) : ICommand<Guid>;

public class UpdateAuthorRoleCommandValidator : AbstractValidator<UpdateAuthorRoleCommand>
{
    public UpdateAuthorRoleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(MessageCode.RoleNotFound);
    }
}

public class UpdateAuthorRoleCommandHandler(IDocumentSession session) : IRequestHandler<UpdateAuthorRoleCommand, Guid>
{
    public async Task<Guid> Handle(UpdateAuthorRoleCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<AuthorRoleEntity>(request.Id, cancellationToken);
        if (entity == null)
            throw new ClientValidationException(MessageCode.RoleNotFound, request.Id);

        var name = request.Dto.Name?.Trim() ?? entity.Name;
        entity.Update(name, request.Dto.Description?.Trim());

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

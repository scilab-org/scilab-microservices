using Lab.Application.Dtos.AuthorRoles;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.AuthorRole.Commands.CreateAuthorRole;

public record CreateAuthorRoleCommand(CreateAuthorRoleDto Dto) : ICommand<Guid>;

public class CreateAuthorRoleCommandValidator : AbstractValidator<CreateAuthorRoleCommand>
{
    public CreateAuthorRoleCommandValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage(MessageCode.RoleNamesAreRequired)
            .NotNull().WithMessage(MessageCode.RoleNamesAreRequired);
    }
}

public class CreateAuthorRoleCommandHandler(IDocumentSession session) : IRequestHandler<CreateAuthorRoleCommand, Guid>
{
    public async Task<Guid> Handle(CreateAuthorRoleCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var normalizedName = request.Dto.Name.Trim();
        var existing = await session.Query<AuthorRoleEntity>()
            .FirstOrDefaultAsync(x => x.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (existing != null)
            throw new ClientValidationException(MessageCode.AuthorRoleNameAlreadyExists, request.Dto.Name);

        var entity = AuthorRoleEntity.Create(Guid.NewGuid(), normalizedName, request.Dto.Description);
        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

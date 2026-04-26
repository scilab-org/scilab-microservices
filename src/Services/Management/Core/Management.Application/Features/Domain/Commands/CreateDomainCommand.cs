using Management.Application.Dtos.Domains;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Domain.Commands;

public sealed record CreateDomainCommand(CreateDomainDto Dto) : ICommand<Guid>;

[ExcludeFromCodeCoverage]
public sealed class CreateDomainCommandValidator : AbstractValidator<CreateDomainCommand>
{
    #region Ctors

    public CreateDomainCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.DomainNameIsRequired);
            });
    }

    #endregion
}


public sealed class CreateDomainCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateDomainCommand, Guid>
{
    public async Task<Guid> Handle(CreateDomainCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var entity = DomainEntity.Create(id, command.Dto.Name!);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

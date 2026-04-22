using Management.Application.Dtos.Domains;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Domain.Commands;

public sealed record UpdateDomainCommand(Guid DomainId, UpdateDomainDto Dto) : ICommand<Guid>;

public sealed class UpdateDomainCommandValidator : AbstractValidator<UpdateDomainCommand>
{
    #region Ctors

    public UpdateDomainCommandValidator()
    {
        RuleFor(x => x.DomainId)
            .NotEmpty()
            .WithMessage(MessageCode.DomainIdIsRequired);

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

public sealed class UpdateDomainCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateDomainCommand, Guid>
{
    public async Task<Guid> Handle(UpdateDomainCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<DomainEntity>(command.DomainId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.DomainIsNotExists, command.DomainId.ToString());

        entity.Update(command.Dto.Name!, command.Dto.Description);
        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

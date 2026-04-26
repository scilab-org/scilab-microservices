using Management.Application.Dtos.Affiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Affiliation.Commands;

public sealed record UpdateAffiliationCommand(Guid Id, UpdateAffiliationDto Dto) : ICommand<Guid>;

[ExcludeFromCodeCoverage]
public sealed class UpdateAffiliationCommandValidator : AbstractValidator<UpdateAffiliationCommand>
{
    public UpdateAffiliationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.BadRequest);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class UpdateAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateAffiliationCommand, Guid>
{
    public async Task<Guid> Handle(UpdateAffiliationCommand command, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<AffiliationEntity>(command.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.AffiliationIsNotExists, command.Id.ToString());

        entity.Update(command.Dto.Name, command.Dto.ShortName, command.Dto.RorId, command.Dto.RorUrl);
        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}

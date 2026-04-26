using Management.Application.Dtos.Affiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Affiliation.Commands;

public sealed record CreateAffiliationCommand(CreateAffiliationDto Dto) : ICommand<Guid>;

[ExcludeFromCodeCoverage]
public sealed class CreateAffiliationCommandValidator : AbstractValidator<CreateAffiliationCommand>
{
    public CreateAffiliationCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.AffiliationNameIsRequired);

                RuleFor(x => x.Dto.RorId)
                    .NotEmpty()
                    .WithMessage(MessageCode.BadRequest);
            });
    }
}

public sealed class CreateAffiliationCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateAffiliationCommand, Guid>
{
    public async Task<Guid> Handle(CreateAffiliationCommand command, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var entity = AffiliationEntity.Create(id, command.Dto.Name!, command.Dto.ShortName, command.Dto.RorId!, command.Dto.RorUrl);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

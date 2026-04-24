using Lab.Application.Dtos.GapTypes;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.GapType.Commands.CreateGapType;

public record CreateGapTypeCommand(CreateGapTypeDto Dto) : ICommand<Guid>;

public class CreateGapTypeCommandValidator : AbstractValidator<CreateGapTypeCommand>
{
    public CreateGapTypeCommandValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .WithMessage(MessageCode.BadRequest);
    }
}

public class CreateGapTypeCommandHandler(IDocumentSession session) : IRequestHandler<CreateGapTypeCommand, Guid>
{
    public async Task<Guid> Handle(CreateGapTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = GapTypeEntity.Create(Guid.NewGuid(), request.Dto.Name.Trim());

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

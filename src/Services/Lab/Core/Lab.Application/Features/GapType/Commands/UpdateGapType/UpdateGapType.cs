using Lab.Application.Dtos.GapTypes;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.GapType.Commands.UpdateGapType;

public record UpdateGapTypeCommand(Guid Id, UpdateGapTypeDto Dto) : ICommand<Guid>;

public class UpdateGapTypeCommandValidator : AbstractValidator<UpdateGapTypeCommand>
{
    public UpdateGapTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class UpdateGapTypeCommandHandler(IDocumentSession session) : IRequestHandler<UpdateGapTypeCommand, Guid>
{
    public async Task<Guid> Handle(UpdateGapTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<GapTypeEntity>(request.Id, cancellationToken);
        if (entity == null)
            throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        var name = request.Dto.Name?.Trim() ?? entity.Name;
        entity.Update(name);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.GapType.Commands.DeleteGapType;

public record DeleteGapTypeCommand(Guid Id) : ICommand<Guid>;

public class DeleteGapTypeCommandValidator : AbstractValidator<DeleteGapTypeCommand>
{
    public DeleteGapTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteGapTypeCommandHandler(IDocumentSession session) : IRequestHandler<DeleteGapTypeCommand, Guid>
{
    public async Task<Guid> Handle(DeleteGapTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<GapTypeEntity>(request.Id, cancellationToken);
        if (entity == null)
            throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}

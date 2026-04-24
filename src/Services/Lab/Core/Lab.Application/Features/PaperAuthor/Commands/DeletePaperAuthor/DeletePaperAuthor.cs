using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperAuthor.Commands.DeletePaperAuthor;

public record DeletePaperAuthorCommand(Guid Id) : ICommand<Unit>;

public class DeletePaperAuthorCommandValidator : AbstractValidator<DeletePaperAuthorCommand>
{
    public DeletePaperAuthorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeletePaperAuthorCommandHandler(IDocumentSession session) : IRequestHandler<DeletePaperAuthorCommand, Unit>
{
    public async Task<Unit> Handle(DeletePaperAuthorCommand request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<PaperAuthorEntity>(request.Id, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        session.Delete(entity);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

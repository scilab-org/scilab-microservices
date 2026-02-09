using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Commands.DeletePaper;

public record DeletePaperCommand(Guid Id) : ICommand<Unit>;

public class DeletePaperCommandValidator : AbstractValidator<DeletePaperCommand>
{
    public DeletePaperCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class DeletePaperCommandHandler(IDocumentSession session) : IRequestHandler<DeletePaperCommand, Unit>
{
    public async Task<Unit> Handle(DeletePaperCommand request, CancellationToken cancellationToken)
    {
        var dataset = await session.LoadAsync<PaperEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id.ToString());

        session.Delete(dataset);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
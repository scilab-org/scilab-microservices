using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Tag.Commands.DeleteTag;

public record DeleteTagCommand(Guid Id) : ICommand<Unit>;

public class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.TagIdIsRequired);
    }
}

public class DeleteTagCommandHandler(IDocumentSession session) : IRequestHandler<DeleteTagCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var dataset = await session.LoadAsync<TagEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.TagIsNotExists, request.Id.ToString());

        session.Delete(dataset);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
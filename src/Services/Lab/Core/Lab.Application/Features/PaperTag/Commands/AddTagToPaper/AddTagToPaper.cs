using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperTag.Commands.AddTagToPaper;

public record AddTagToPaperCommand(Guid PaperId, List<Guid> Tags) : ICommand<Guid>;


public class AddTagToPaperCommandValidator : AbstractValidator<AddTagToPaperCommand>
{
    public AddTagToPaperCommandValidator()
    {
            RuleFor(x => x.PaperId).NotEmpty().WithMessage(MessageCode.PaperIdIsRequired);
            RuleFor(x => x.Tags).NotEmpty().WithMessage(MessageCode.PaperTagIdIsRequired);
    }
}

public class AddTagToPaperCommandHandler(IDocumentSession session) : IRequestHandler<AddTagToPaperCommand, Guid>
{
    public async Task<Guid> Handle(AddTagToPaperCommand request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken);
        if (paper == null)
        {
            throw new NotFoundException(MessageCode.PaperIsNotExists);
        }

        return paper.Id;
    }
}
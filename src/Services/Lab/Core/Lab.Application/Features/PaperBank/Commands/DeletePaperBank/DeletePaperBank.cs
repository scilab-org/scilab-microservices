using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperBank.Commands.DeletePaperBank;

public record DeletePaperBankCommand(Guid Id) : ICommand<Unit>;

public class DeletePaperBankCommandValidator : AbstractValidator<DeletePaperBankCommand>
{
    public DeletePaperBankCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);
    }
}

public class DeletePaperBankCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : IRequestHandler<DeletePaperBankCommand, Unit>
{
    public async Task<Unit> Handle(DeletePaperBankCommand request, CancellationToken cancellationToken)
    {
        var paper = await session.LoadAsync<PaperBankEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id.ToString());

        await managementApiService.DeleteProjectPaperByBankIdAsync(request.Id, cancellationToken);
        
        session.Delete(paper);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

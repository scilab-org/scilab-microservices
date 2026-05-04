using JasperFx.Core;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.CheckList.Commands.DeleteCheckList;

public record DeleteCheckListCommand(Guid Id, string UserName) : ICommand<Unit>;

public class DeleteCheckListCommandValidator : AbstractValidator<DeleteCheckListCommand>
{
    public DeleteCheckListCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.CheckListIdIsRequired);
    }
}

public class DeleteCheckListCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteCheckListCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCheckListCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var checkList = await session.LoadAsync<CheckListEntity>(request.Id, cancellationToken)
                        ?? throw new ClientValidationException(MessageCode.CheckListIsNotExists, request.Id.ToString());

        checkList.Update(modifiedBy: request.UserName);

        session.Update(checkList);
        session.Delete(checkList);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
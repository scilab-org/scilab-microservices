using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperContributor.Commands.DeletePaperContributor;

public sealed record DeletePaperContributorCommand(Guid Id) : ICommand<Unit>;


public class DeletePaperContributorCommandHandler(IDocumentSession session)
    : ICommandHandler<DeletePaperContributorCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeletePaperContributorCommand command, CancellationToken cancellationToken)
    {
        var current = await session.LoadAsync<PaperContributorEntity>(command.Id, cancellationToken);
        if (current is null)
            throw new NotFoundException(MessageCode.PaperContributorNotFound);

        session.Delete(current);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}

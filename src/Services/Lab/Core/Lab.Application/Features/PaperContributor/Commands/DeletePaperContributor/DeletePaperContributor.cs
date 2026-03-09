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
        var template = await session.LoadAsync<PaperContributorEntity>(command.Id, cancellationToken);
        if (template is null)
            throw new NotFoundException($"Template with id {command.Id} not found.");

        session.Delete(template);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}

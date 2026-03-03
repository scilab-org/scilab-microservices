using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Template.Commands;

public sealed record DeleteTemplateCommand(Guid Id) : ICommand<Unit>;

public class DeleteTemplateCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteTemplateCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
    {
        var template = await session.LoadAsync<TemplateEntity>(command.Id, cancellationToken);
        if (template is null)
            throw new NotFoundException($"Template with id {command.Id} not found.");

        session.Delete(template);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}


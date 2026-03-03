using Lab.Application.Dtos.Template;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Template.Commands;

public sealed record UpdateTemplateCommand(Guid Id, CreateTemplateVersionDto Dto) : ICommand<Guid>;

public class CreateTemplateVersionCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateTemplateCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateTemplateCommand command, CancellationToken cancellationToken)
    {
        var current = await session.LoadAsync<TemplateEntity>(command.Id, cancellationToken);
        if (current == null)
            throw new ("Template not found.");
        
        current.Update(
            description: command.Dto.Description ?? current.Description,
            templateStructure: command.Dto.TemplateStructure
        );
        session.Store(current);
        
        await session.SaveChangesAsync(cancellationToken);

        return current.Id;
        
    }

    #endregion
}
using Lab.Application.Dtos.Template;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Template.Commands;

public sealed record CreateTemplateCommand(CreateTemplateDto Dto) : ICommand<Guid>;

public class CreatePaperTemplateCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateTemplateCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;
        
        var exists = await session.Query<TemplateEntity>()
            .AnyAsync(x => x.Code == dto.Code);

        if (exists)
            throw new ("Template code already exists.");
        
        var template = TemplateEntity.Create(
            id: Guid.NewGuid(),
            name: dto.Name,
            code: dto.Code,
            description: dto.Description,
            templateStructure: dto.TemplateStructure
            );
        
        session.Store(template);
        await session.SaveChangesAsync(cancellationToken);

        return template.Id;
    }

    #endregion
}
using Lab.Application.Dtos.Template;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Template.Commands;

public sealed record CreateTemplateCommand(CreateTemplateDto Dto) : ICommand<Guid>;

public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.TemplateIsRequired)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Code)
                    .NotEmpty()
                    .WithMessage(MessageCode.TemplateCodeIsRequired);

                RuleFor(x => x.Dto.Sections)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage(MessageCode.TemplateSectionsAreRequired);

                RuleForEach(x => x.Dto.Sections)
                    .NotNull()
                    .WithMessage(MessageCode.SectionIsRequired)
                    .ChildRules(section =>
                    {
                        section.RuleFor(x => x.Title)
                            .NotEmpty()
                            .WithMessage(MessageCode.SectionTitleIsRequired);

                        section.RuleFor(x => x.SectionRule)
                            .NotEmpty()
                            .WithMessage(MessageCode.SectionRuleIsRequired);

                        section.RuleFor(x => x.DisplayOrder)
                            .NotEmpty()
                            .WithMessage(MessageCode.SectionDisplayOrderIsRequired);
                    });
            });
    }
}

public class CreatePaperTemplateCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateTemplateCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var exists = await session.Query<TemplateEntity>()
            .AnyAsync(x => x.Code == dto.Code, token: cancellationToken);

        if (exists)
            throw new ClientValidationException(MessageCode.TemplateCodeIsAlreadyExists, dto.Code);

        var template = TemplateEntity.Create(
            code: dto.Code,
            description: dto.Description,
            sections: dto.Sections
            );

        session.Store(template);
        await session.SaveChangesAsync(cancellationToken);

        return template.Id;
    }

    #endregion
}
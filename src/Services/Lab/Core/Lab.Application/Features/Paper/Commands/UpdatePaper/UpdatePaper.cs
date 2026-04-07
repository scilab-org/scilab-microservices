using Lab.Application.Dtos.Papers;
using Lab.Application.Rules;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using DomainRules = Lab.Domain.Constants.Rules;

namespace Lab.Application.Features.Paper.Commands.UpdatePaper;
public record UpdatePaperCommand(UpdatePaperDto Dto, Guid Id, Guid UserId) : ICommand<Guid>;

public class UpdatePaperCommandValidator : AbstractValidator<UpdatePaperCommand>
{
    public UpdatePaperCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Context)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperContextIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperContextIsRequired);
                RuleFor(x => x.Dto.Journal)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperJournalIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperJournalIsRequired);
            });
    }
}

public class UpdatePaperCommandHandler(
    IDocumentSession session) : ICommandHandler<UpdatePaperCommand, Guid>
{
    #region Implementation

    public async Task<Guid> Handle(UpdatePaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var paper = await session.Query<PaperEntity>()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(MessageCode.PaperIsNotExists, request.Id.ToString());

        await session.BeginTransactionAsync(cancellationToken);
        
        paper.Update(
            context: dto.Context,
            abstractText: dto.Abstract,
            researchGap: dto.ResearchGap,
            mainContribution: dto.MainContribution,
            gapType: dto.GapType,
            journal: dto.Journal.Name,
            styleName: dto.Journal.StyleName,
            styleDescription: dto.Journal.StyleDescription,
            styleRule: dto.Journal.StyleRule,
            rule: DomainRules.Paper,
            status: dto.Status ?? PaperStatus.Processing
        );

        var paperRule = SectionRuleComposer.BuildPaperRule(paper);
        var sections = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == paper.Id)
            .ToListAsync(cancellationToken);

        foreach (var section in sections)
        {
            var sectionRule = SectionRuleComposer.BuildSectionRule(section.Title, section.Description);
            var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(
                section.ProjectRule,
                paperRule,
                sectionRule);

            section.Update(
                paperRule: paperRule,
                sectionRule: sectionRule,
                rule: normalizedRule);

            session.Update(section);
        }

        session.Store(paper);
        await session.SaveChangesAsync(cancellationToken);

        return paper.Id;
    }

    #endregion
}
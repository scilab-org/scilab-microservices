using Lab.Application.Dtos.Papers;
using Lab.Application.Rules;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using DomainRules = Lab.Domain.Constants.Rules;

namespace Lab.Application.Features.Paper.Commands.UpdatePaper;

public record UpdatePaperCommand(UpdatePaperDto Dto, Guid Id, string UserName) : ICommand<Guid>;

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

        var gapTypeIds = (dto.GapTypeIds ?? [])
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var gapTypes = gapTypeIds.Count == 0
            ? []
            : await session.Query<GapTypeEntity>()
                .Where(x => gapTypeIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        if (gapTypes.Count != gapTypeIds.Count)
        {
            var foundIds = gapTypes.Select(x => x.Id).ToHashSet();
            var missingIds = gapTypeIds.Where(id => !foundIds.Contains(id)).ToList();
            throw new NotFoundException(MessageCode.GapTypeIsNotExists, string.Join(", ", missingIds));
        }

        var gapTypeNames = gapTypes
            .Select(x => x.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        paper.Update(
            context: dto.Context,
            abstractText: dto.Abstract,
            researchGap: dto.ResearchGap,
            mainContribution: dto.MainContribution,
            researchAim: dto.ResearchAim,
            gapTypeIds: gapTypeIds,
            rule: DomainRules.Paper,
            conferenceJournalStartAt: dto.ConferenceJournalStartAt,
            conferenceJournalEndAt: dto.ConferenceJournalEndAt,
            lastModifiedBy: request.UserName
        );

        var journal = await session.LoadAsync<ConferenceJournalEntity>(paper.ConferenceJournalId.Value, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.JournalIsNotExists,
                          paper.ConferenceJournalId.ToString());

        var paperRule = SectionRuleComposer.BuildPaperRule(paper, gapTypeNames, journal);
        var paperContext = SectionRuleComposer.BuildPaperContext(paper);
        var sections = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == paper.Id)
            .ToListAsync(cancellationToken);

        foreach (var section in sections)
        {
            var sectionRule =
                SectionRuleComposer.BuildSectionRule(section.Title, section.Description, section.MainIdea);
            var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(
                section.ProjectRule,
                paperRule,
                sectionRule);

            section.Update(
                paperRule: paperRule,
                sectionRule: sectionRule,
                rule: normalizedRule,
                paperContext: paperContext,
                sectionContext: SectionRuleComposer.ComposeSectionContext(
                    section.ProjectContext, paperContext, section.Title, section.MainIdea));

            session.Update(section);
        }

        session.Store(paper);
        await session.SaveChangesAsync(cancellationToken);

        return paper.Id;
    }

    #endregion
}
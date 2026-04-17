using Lab.Application.Dtos.Projects;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.System;

public sealed record UpdateProjectRulesCommand(UpdateProjectRulesDto Dto) : ICommand<bool>;

public sealed class UpdateProjectRulesCommandValidator : AbstractValidator<UpdateProjectRulesCommand>
{
    public UpdateProjectRulesCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest);
    }
}

public sealed class UpdateProjectRulesCommandHandler(
    IDocumentSession session) : ICommandHandler<UpdateProjectRulesCommand, bool>
{
    public async Task<bool> Handle(UpdateProjectRulesCommand request, CancellationToken cancellationToken)
    {
        var paperIds = request.Dto.PaperIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (paperIds.Count == 0)
            return true;

        await session.BeginTransactionAsync(cancellationToken);

        var papers = await session.Query<PaperEntity>()
            .Where(x => paperIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (papers.Count == 0)
            return true;

        var paperMap = papers.ToDictionary(x => x.Id);
        var sectionPaperIds = paperMap.Keys.ToList();

        var sections = await session.Query<SectionEntity>()
            .Where(x => sectionPaperIds.Contains(x.PaperId))
            .ToListAsync(cancellationToken);

        if (sections.Count == 0)
            return true;

        var projectRule = SectionRuleComposer.BuildProjectRule(new ManagementProjectInfo(
            Guid.Empty,
            null,
            null,
            null,
            null,
            null,
            null,
            request.Dto.Context,
            request.Dto.Domain,
            request.Dto.Keypoint));

        var projectContext = SectionRuleComposer.BuildProjectContext(new ManagementProjectInfo(
            Guid.Empty,
            null,
            null,
            null,
            null,
            null,
            null,
            request.Dto.Context,
            request.Dto.Domain,
            request.Dto.Keypoint));

        foreach (var section in sections)
        {
            if (!paperMap.TryGetValue(section.PaperId, out var paper))
                continue;

            var journal = await session.LoadAsync<ConferenceJournalEntity>(paper.ConferenceJournalId!, cancellationToken)
                          ?? throw new NotFoundException(MessageCode.JournalIsNotExists,
                              paper.ConferenceJournalId.ToString());

            var paperRule = string.IsNullOrWhiteSpace(section.PaperRule)
                ? SectionRuleComposer.BuildPaperRule(paper, journal)
                : section.PaperRule;

            var sectionRule = string.IsNullOrWhiteSpace(section.SectionRule)
                ? SectionRuleComposer.BuildSectionRule(section.Title, section.Description, section.MainIdea)
                : section.SectionRule;

            var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(
                projectRule,
                paperRule,
                sectionRule);

            section.Update(
                projectRule: projectRule,
                paperRule: paperRule,
                sectionRule: sectionRule,
                rule: normalizedRule,
                projectContext: projectContext,
                sectionContext: SectionRuleComposer.ComposeSectionContext(
                    projectContext, section.PaperContext, section.Title, section.MainIdea));

            session.Update(section);
        }

        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}
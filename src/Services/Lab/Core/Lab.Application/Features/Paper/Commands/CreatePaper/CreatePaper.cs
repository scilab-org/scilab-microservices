using Lab.Application.Dtos.Papers;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using DomainRules = Lab.Domain.Constants.Rules;

namespace Lab.Application.Features.Paper.Commands.CreatePaper;

public record CreatePaperCommand(CreatePaperDto Dto, Guid UserId, string UserName) : ICommand<Guid>;

public class CreatePaperCommandValidator : AbstractValidator<CreatePaperCommand>
{
    public CreatePaperCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Title)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperTitleIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperTitleIsRequired);
                RuleFor(x => x.Dto.Template)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperTemplateIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperTemplateIsRequired);
                RuleFor(x => x.Dto.Context)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperContextIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperContextIsRequired);
                RuleFor(x => x.Dto.ResearchGap)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperResearchGapIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperResearchGapIsRequired);
                RuleFor(x => x.Dto.GapType)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperGapTypeIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperGapTypeIsRequired);
                RuleFor(x => x.Dto.MainContribution)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperMainContributionIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperMainContributionIsRequired);
                RuleFor(x => x.Dto.ResearchAim)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperResearchAimIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperResearchAimIsRequired);
                RuleFor(x => x.Dto.ConferenceJournalId)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalIdIsRequired);
                RuleFor(x => x.Dto.ConferenceJournalName)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalNameIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalNameIsRequired);
                RuleFor(x => x.Dto.ConferenceJournalStartAt)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalStartAtIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalStartAtIsRequired);
                RuleFor(x => x.Dto.ConferenceJournalEndAt)
                    .NotEmpty()
                    .WithMessage(MessageCode.JournalEndAtIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.JournalEndAtIsRequired);
            });
    }
}

public class CreatePaperCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : ICommandHandler<CreatePaperCommand, Guid>
{
    #region Implementation

    public async Task<Guid> Handle(CreatePaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var isAtuhor = await managementApiService.GetMyProjectRoleAsync(dto.ProjectId, cancellationToken);
        if (dto.ProjectId != Guid.Empty && isAtuhor != AuthorizeConstants.ProjectAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);

        await session.BeginTransactionAsync(cancellationToken);

        var project = await managementApiService.GetProjectByIdAsync(dto.ProjectId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.ProjectIsNotExists, dto.ProjectId.ToString());

        var journal = await session.LoadAsync<ConferenceJournalEntity>(dto.ConferenceJournalId, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.JournalIsNotExists,
                          dto.ConferenceJournalId.ToString());
        var paperId = Guid.NewGuid();

        var projectIds = journal.ProjectIds ?? [];
        projectIds.Add(dto.ProjectId);
        projectIds = projectIds.Distinct().ToList();

        var paperIds = journal.PaperIds ?? [];
        paperIds.Add(paperId);
        paperIds = paperIds.Distinct().ToList();

        journal.Update(projectIds: projectIds, paperIds: paperIds);
        session.Update(journal);

        var projectRule = SectionRuleComposer.BuildProjectRule(project);
        var paperRule = SectionRuleComposer.BuildPaperRule(dto, journal);
        var projectContext = SectionRuleComposer.BuildProjectContext(project);
        var paperContext = SectionRuleComposer.BuildPaperContext(dto);

        var entity = PaperEntity.Create(
            id: paperId,
            title: dto.Title,
            template: dto.Template,
            context: dto.Context,
            abstractText: dto.Abstract,
            researchGap: dto.ResearchGap,
            mainContribution: dto.MainContribution,
            researchAim: dto.ResearchAim,
            gapType: dto.GapType,
            conferenceJournalName: dto.ConferenceJournalName,
            conferenceJournalId: dto.ConferenceJournalId,
            conferenceJournalStartAt: dto.ConferenceJournalStartAt,
            conferenceJournalEndAt: dto.ConferenceJournalEndAt,
            rule: DomainRules.Paper,
            status: PaperStatus.Draft,
            createdBy: request.UserName
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
        {
            foreach (var template in dto.Sections)
            {
                var sectionRule =
                    SectionRuleComposer.BuildSectionRule(template.Title, template.SectionRule, template.MainIdea);
                var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(projectRule, paperRule, sectionRule);
                var sectionContext = SectionRuleComposer.ComposeSectionContext(projectContext, paperContext, template.Title, template.MainIdea);

                var section = SectionEntity.Create(
                    id: Guid.NewGuid(),
                    content: "",
                    title: template.Title,
                    description: template.SectionRule,
                    status: SectionStatus.NotStarted,
                    mainIdea: template.MainIdea,
                    rule: normalizedRule,
                    displayOrder: template.DisplayOrder,
                    isMainSection: true,
                    version: "Version Initial",
                    paperId: entity.Id,
                    createdBy: request.UserName,
                    paperRule: paperRule,
                    projectRule: projectRule,
                    sectionRule: sectionRule,
                    packages: ["\\documentclass{article}", "\\usepackage{biblatex}"],
                    sectionContext: sectionContext,
                    projectContext: projectContext,
                    paperContext: paperContext
                );
                session.Store(section);
            }
        }

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        if (dto.ProjectId != Guid.Empty)
        {
            var subProjectId = await managementApiService.CreateSubProjectAsync(
                dto.ProjectId, entity.Id, dto.Title, cancellationToken);

            if (subProjectId.HasValue)
            {
                await managementApiService.AddSubProjectMembersAsync(
                    subProjectId.Value,
                    [(request.UserId, AuthorizeConstants.PaperAuthor)],
                    cancellationToken);
            }

            var result = await managementApiService.AddProjectConferenceJournalsAsync(
                dto.ProjectId, dto.ConferenceJournalId, cancellationToken);

            if (!result)
                throw new NotFoundException(MessageCode.ProjectIsNotExists, dto.ProjectId.ToString());
        }

        return entity.Id;
    }

    #endregion
}
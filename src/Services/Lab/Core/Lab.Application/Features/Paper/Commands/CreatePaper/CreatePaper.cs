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
                RuleFor(x => x.Dto.Context)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperContextIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperContextIsRequired);
                RuleFor(x => x.Dto.ConferenceJournalName)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperJournalIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperJournalIsRequired);
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

        var projectIds = journal.ProjectIds ?? [];
        projectIds.Add(dto.ProjectId);
        projectIds = projectIds.Distinct().ToList();
        journal.Update(projectIds: projectIds);
        session.Update(journal);

        var projectRule = SectionRuleComposer.BuildProjectRule(project);
        var paperRule = SectionRuleComposer.BuildPaperRule(dto, journal.Style!);

        var entity = PaperEntity.Create(
            id: Guid.NewGuid(),
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
            rule: DomainRules.Paper,
            status: dto.Status ?? PaperStatus.Processing,
            createdBy: request.UserName
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
        {
            foreach (var template in dto.Sections)
            {
                var sectionRule =
                    SectionRuleComposer.BuildSectionRule(template.Title, template.SectionRule, template.MainIdea);
                var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(projectRule, paperRule, sectionRule);

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
                    packages: []
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
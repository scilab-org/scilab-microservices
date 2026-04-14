using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Sections;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Constants;
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
                RuleFor(x => x.Dto.Journal)
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

        var projectRule = SectionRuleComposer.BuildProjectRule(project);
        var paperRule = SectionRuleComposer.BuildPaperRule(dto);

        var entity = PaperEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            template: dto.Template,
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
            status: dto.Status ?? PaperStatus.Processing,
            createdBy: request.UserName
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
        {
            foreach (var template in dto.Sections)
            {
                var sectionRule = SectionRuleComposer.BuildSectionRule(template.Title, template.Description);
                var normalizedRule = SectionRuleComposer.ComposeNormalizedRule(projectRule, paperRule, sectionRule);

                var section = SectionEntity.Create(
                    id: template.Id,
                    content: template.Content,
                    title: template.Title,
                    sectionSumary: template.SectionSumary,
                    description: template.Description,
                    rule: normalizedRule,
                    displayOrder: template.DisplayOrder,
                    numbered: template.Numbered,
                    isMainSection: true,
                    version: "Version Initial",
                    paperId: entity.Id,
                    parentSectionId: template.ParentSectionId,
                    createdBy: request.UserName,
                    paperRule: paperRule,
                    projectRule: projectRule,
                    sectionRule: sectionRule,
                    packages: template.Packages
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
                    new[] { (request.UserId, AuthorizeConstants.PaperAuthor) },
                    cancellationToken);
            }
        }

        return entity.Id;
    }

    #endregion
}
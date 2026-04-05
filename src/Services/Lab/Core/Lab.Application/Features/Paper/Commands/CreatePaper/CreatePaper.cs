using System.Text;
using Lab.Application.Dtos.Papers;
using Lab.Application.Dtos.Sections;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

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
            rule: Rules.Paper,
            status: dto.Status ?? PaperStatus.Processing,
            createdBy: request.UserName
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
        {
            var normalizedSections = NormalizeSection(project, dto, dto.Sections);
            foreach (var template in normalizedSections)
            {
                var section = SectionEntity.Create(
                    id: template.Id,
                    content: template.Content,
                    title: template.Title,
                    sectionSumary: template.SectionSumary,
                    description: template.Description,
                    rule: template.Rule,
                    displayOrder: template.DisplayOrder,
                    numbered: template.Numbered,
                    isMainSection: true,
                    paperId: entity.Id,
                    parentSectionId: template.ParentSectionId,
                    createdBy: request.UserName
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

    #region Methods

    /// <summary>
    /// Normalizes sections by combining project context, paper information, and section rules
    /// into a consolidated markdown-formatted content structure.
    /// </summary>
    private List<CreateSectionDto> NormalizeSection(
        ManagementProjectInfo project,
        CreatePaperDto paperDto,
        List<CreateSectionDto> sections)
    {
        if (sections.Count == 0)
            return [];

        var normalizedSections = new List<CreateSectionDto>();

        foreach (var section in sections)
        {
            var normalizedRule = BuildNormalizedRule(project, paperDto, section);

            var normalizedSection = new CreateSectionDto
            {
                Id = section.Id,
                Title = section.Title,
                Content = section.Content,
                Numbered = section.Numbered,
                DisplayOrder = section.DisplayOrder,
                SectionSumary = section.SectionSumary,
                Description = section.Description,
                Rule = normalizedRule,
                ParentSectionId = section.ParentSectionId
            };

            normalizedSections.Add(normalizedSection);
        }

        return normalizedSections;
    }

    /// <summary>
    /// Builds normalized rule by combining project context, paper guidelines, and section rules
    /// with Markdown formatting.
    /// </summary>
    private string BuildNormalizedRule(
        ManagementProjectInfo project,
        CreatePaperDto paperDto,
        CreateSectionDto section)
    {
        var contentBuilder = new StringBuilder();

        contentBuilder.AppendLine("## Rule Level to write research paper");
        contentBuilder.AppendLine("**Level 1 (Critical):** Must follow strictly");
        contentBuilder.AppendLine("**Level 2 (Important):** Should follow");
        contentBuilder.AppendLine("**Level 3 (Guidelines):** Consider Context");

        // Project
        contentBuilder.AppendLine("# Project: **Level 3 (Guildline)**");
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Context");
        if (!string.IsNullOrEmpty(project.Context))
            contentBuilder.AppendLine(project.Context);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Keypoint");
        if (!string.IsNullOrEmpty(project.Keypoint))
            contentBuilder.AppendLine(project.Keypoint);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Domain");
        if (!string.IsNullOrEmpty(project.Domain))
            contentBuilder.AppendLine(project.Domain);
        contentBuilder.AppendLine();

        // Paper
        contentBuilder.AppendLine("# Paper: **Level 2 (Important)**");
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Context");
        if (!string.IsNullOrEmpty(paperDto.Context))
            contentBuilder.AppendLine(paperDto.Context);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Research Gap");
        if (!string.IsNullOrEmpty(paperDto.ResearchGap))
            contentBuilder.AppendLine(paperDto.ResearchGap);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Gap Type");
        if (!string.IsNullOrEmpty(paperDto.GapType))
            contentBuilder.AppendLine(paperDto.GapType);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Rule");
        contentBuilder.AppendLine(Rules.Paper);
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Journal Style");
        if (!string.IsNullOrEmpty(paperDto.Journal.StyleRule))
            contentBuilder.AppendLine(paperDto.Journal.StyleRule);
        contentBuilder.AppendLine();

        // Section
        contentBuilder.AppendLine($"# Section {section.Title}: **Level 1 (Critical)**");
        contentBuilder.AppendLine();

        contentBuilder.AppendLine("## Rule");
        if (!string.IsNullOrEmpty(section.Rule))
            contentBuilder.AppendLine(section.Rule);
        contentBuilder.AppendLine();

        return contentBuilder.ToString();
    }

    #endregion
}
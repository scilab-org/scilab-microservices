using JasperFx.Core;
using Lab.Application.Rules;
using Lab.Application.Dtos.Sections;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Section.Commands.UpsertSection;

public record UpsertSectionCommand(UpsertSectionDto Dto, Guid Id, string UserName) : ICommand<Guid>;

public class UpsertSectionCommandValidator : AbstractValidator<UpsertSectionCommand>
{
    public UpsertSectionCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.MemberId)
                    .NotEmpty()
                    .WithMessage(MessageCode.MemberIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.MemberIdIsRequired);

                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage(MessageCode.SectionIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.SectionIdIsRequired);
            });
    }
}

public class UpsertSectionCommandHandler(
    IDocumentSession session) : ICommandHandler<UpsertSectionCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpsertSectionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, request.Id);

        var contributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == section.PaperId &&
                        x.MemberId == dto.MemberId &&
                        x.SectionId == section.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (contributor == null || contributor.SectionRole.EqualsIgnoreCase(AuthorizeConstants.SectionRead))
            throw new UnauthorizedException(MessageCode.AccessDenied);

        // If the section is main, the section will be created as a new version of main section, and the new section will be updated by writer
        if (section.IsMainSection == true)
        {
            // Prevent writer from creating multiple versions of the same main section
            var availableSection = await session.Query<SectionEntity>()
                .Where(x => x.PaperId == section.PaperId &&
                            x.IsMainSection == false &&
                            x.PreviousVersionSectionId == section.Id)
                .ToListAsync(cancellationToken);
            if (availableSection.Any())
            {
                var contributorWithVersion = await session.Query<PaperContributorEntity>()
                    .Where(x => x.PaperId == section.PaperId &&
                                x.MemberId == dto.MemberId &&
                                x.SectionId == availableSection.Select(s => s.Id).FirstOrDefault())
                    .FirstOrDefaultAsync(cancellationToken);
                if (contributorWithVersion != null)
                    throw new ClientValidationException(MessageCode.SectionAlreadyHasVersion, section.Id);
            }

            if (section.Version.EqualsIgnoreCase("Version Initial"))
                section.Update(status: SectionStatus.InProgress);
            
            var version = BuildNextDraftVersion(section.Version);
            var newSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: dto.Content,
                paperId: section.PaperId,
                displayOrder: section.DisplayOrder,
                status: SectionStatus.InProgress,
                isMainSection: false,
                isOldMainSection: false,
                version: version,
                title: dto.Title,
                sectionSumary: dto.SectionSumary,
                description: section.Description,
                mainIdea: section.MainIdea,
                rule: section.Rule,
                //Mark new section as new version of main section
                previousVersionSectionId: section.Id,
                createdBy: request.UserName,
                paperRule: section.PaperRule,
                projectRule: section.ProjectRule,
                sectionRule: SectionRuleComposer.BuildSectionRule(dto.Title, section.Description, dto.MainIdea ?? section.MainIdea),
                packages: dto.CurrentSectionPackages
            );

            // Find reference section and create a new version with updated packages
            if (dto.ReferencesPackages.Any())
                await UpsertReferenceSectionAsync(section.PaperId, dto.MemberId, dto.ReferencesPackages, request.UserName, cancellationToken);

            // Update contributor to point to new section
            contributor.Update(
                sectionId: newSection.Id,
                markSectionId: section.Id
            );
            
            session.Update(section);
            session.Store(newSection);
            session.Update(contributor);
            await session.SaveChangesAsync(cancellationToken);
            return newSection.Id;
        }

        // If the section is not main section, it means the section is already a new version of main section, so just update the section directly
        var sectionRule = SectionRuleComposer.BuildSectionRule(dto.Title ?? section.Title, section.Description, dto.MainIdea ?? section.MainIdea);
        section.Update(
            content: dto.Content,
            title: dto.Title,
            sectionSumary: dto.SectionSumary,
            status: SectionStatus.InProgress,
            mainIdea: dto.MainIdea,
            sectionRule: sectionRule,
            packages: dto.CurrentSectionPackages,
            rule: SectionRuleComposer.ComposeNormalizedRule(section.ProjectRule, section.PaperRule, sectionRule),
            lastModifiedBy: request.UserName
        );

        // Find reference section and update its packages directly
        if (dto.ReferencesPackages.Any())
            await UpsertReferenceSectionAsync(section.PaperId, dto.MemberId, dto.ReferencesPackages, request.UserName, cancellationToken);

        session.Update(section);
        await session.SaveChangesAsync(cancellationToken);
        return section.Id;
    }

    private async Task UpsertReferenceSectionAsync(
        Guid paperId,
        Guid memberId,
        List<string> referencesPackages,
        string createdBy,
        CancellationToken cancellationToken)
    {
        var refContributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == paperId && x.MemberId == memberId)
            .ToListAsync(cancellationToken);

        var refSectionIds = refContributors
            .Where(x => x.SectionId != null)
            .Select(x => x.SectionId!.Value)
            .ToList();

        var refSection = (await session.Query<SectionEntity>()
            .Where(x => refSectionIds.Contains(x.Id))
            .ToListAsync(cancellationToken))
            .FirstOrDefault(x => x.Title != null && x.Title.ToLower().Contains("reference"));

        if (refSection == null) return;

        var refContributor = refContributors.FirstOrDefault(x => x.SectionId == refSection.Id);

        if (refSection.IsMainSection == true)
        {
            var newRefSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: refSection.Content,
                paperId: refSection.PaperId,
                displayOrder: refSection.DisplayOrder,
                status: SectionStatus.InProgress,
                isMainSection: false,
                isOldMainSection: false,
                title: refSection.Title,
                sectionSumary: refSection.SectionSumary,
                description: refSection.Description,
                mainIdea: refSection.MainIdea,
                rule: refSection.Rule,
                previousVersionSectionId: refSection.Id,
                createdBy: createdBy,
                paperRule: refSection.PaperRule,
                projectRule: refSection.ProjectRule,
                sectionRule: refSection.SectionRule,
                packages: referencesPackages
            );
            session.Store(newRefSection);

            if (refContributor != null)
            {
                refContributor.Update(sectionId: newRefSection.Id, markSectionId: refSection.Id);
                session.Update(refContributor);
            }
        }
        else
        {
            refSection.Update(packages: referencesPackages);
            session.Update(refSection);
        }
    }

    private static string BuildNextDraftVersion(string? currentVersion)
    {
        if (currentVersion.IsNullOrEmpty())
            return "Version 1 Draft";

        var normalized = currentVersion.Trim();
        if (normalized.EqualsIgnoreCase("Version Initial") || normalized.EqualsIgnoreCase("Initial"))
            return "Version 1 Draft";

        var numericToken = normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(token => int.TryParse(token, out _));

        if (numericToken != null && int.TryParse(numericToken, out var currentNumber))
            return $"Version {currentNumber + 1} Draft";

        return "Version 1 Draft";
    }

    #endregion
}
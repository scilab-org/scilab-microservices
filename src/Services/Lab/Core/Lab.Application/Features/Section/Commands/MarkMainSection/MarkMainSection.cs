using JasperFx.Core;
using Lab.Application.Dtos.Sections;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using PaperReference = Lab.Domain.Models.Reference;
using Marten;

namespace Lab.Application.Features.Section.Commands.MarkMainSection;

public record MarkMainSectionCommand(MarkMainSectionDto Dto, string UserName, Guid Id) : ICommand<Guid>;

public class MarkMainSectionCommandValidator : AbstractValidator<MarkMainSectionCommand>
{
    public MarkMainSectionCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);
        RuleFor(c => c.Dto.ProjectId)
            .NotNull()
            .WithMessage(MessageCode.ProjectIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class MarkMainSectionCommandHandler(IDocumentSession session, IManagementApiService managementApiService)
    : ICommandHandler<MarkMainSectionCommand, Guid>
{
    public async Task<Guid> Handle(MarkMainSectionCommand request, CancellationToken cancellationToken)
    {
        var role = await managementApiService.GetMyProjectRoleAsync(request.Dto.ProjectId, cancellationToken);
        if (role.IsNullOrEmpty() || !AuthorizeConstants.PaperAuthor.EqualsIgnoreCase(role))
            throw new UnauthorizedException(MessageCode.AccessDenied);

        await session.BeginTransactionAsync(cancellationToken);

        // Find new main section
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, request.Id);

        if (section.IsMainSection == true)
            throw new ClientValidationException(MessageCode.SectionIsAlreadyMainSection, request.Id);

        var count = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == section.PaperId &&
                        x.IsOldMainSection == true &&
                        x.Title!.EqualsIgnoreCase(section.Title!))
            .CountAsync(cancellationToken);

        // Clone main section to new record
        var newMainSection = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: section.Content,
            paperId: section.PaperId,
            displayOrder: section.DisplayOrder,
            numbered: section.Numbered,
            isMainSection: true,
            isOldMainSection: false,
            version: $"Version {count + 1}",
            title: section.Title,
            sectionSumary: section.SectionSumary,
            description: section.Description,
            rule: section.Rule,
            parentSectionId: section.ParentSectionId,
            previousVersionSectionId: section.Id,
            references: section.References,
            createdBy: section.CreatedBy,
            paperRule: section.PaperRule,
            projectRule: section.ProjectRule,
            packages: section.Packages,
            files: section.Files,
            sectionRule: section.SectionRule ?? SectionRuleComposer.BuildSectionRule(section.Title, section.Description)
        );

        section.Update(nextVersionSectionId: newMainSection.Id, isOldMainSection: true);

        // Find contributor to find old main section
        var contributor = await session.Query<PaperContributorEntity>()
                              .Where(x => x.PaperId == section.PaperId && x.SectionId == section.Id)
                              .FirstOrDefaultAsync(cancellationToken)
                          ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, section.Id);

        // Find old main section
        var oldMainSection = await session.LoadAsync<SectionEntity>(contributor.MarkSectionId, cancellationToken)
                             ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired,
                                 contributor.MarkSectionId);

        // Find old contributor to old main section
        var oldMainContributors = await session.Query<PaperContributorEntity>()
                                      .Where(x => x.PaperId == oldMainSection.PaperId &&
                                                  x.SectionId == oldMainSection.Id)
                                      .ToListAsync(cancellationToken)
                                  ?? throw new NotFoundException(MessageCode.ContributorIsNotExists, oldMainSection.Id);

        foreach (var oldMainContributor in oldMainContributors)
        {
            oldMainContributor.Update(markSectionId: section.Id);
            var newMainContributor = PaperContributorEntity.Create(
                id: Guid.NewGuid(),
                sectionRole: oldMainContributor.SectionRole,
                paperId: oldMainContributor.PaperId,
                sectionId: newMainSection.Id,
                memberId: oldMainContributor.MemberId,
                markSectionId: newMainSection.Id // Mark new contributor to new main section
            );
            session.Store(newMainContributor);
            session.Update(oldMainContributor);
        }

        // Update old main section
        oldMainSection.Update(isMainSection: false, nextVersionSectionId: newMainSection.Id);

        // Find others version of main section
        var otherVersionSections = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == section.PaperId &&
                        x.PreviousVersionSectionId == oldMainSection.Id)
            .ToListAsync(cancellationToken);

        foreach (var otherVersionSection in otherVersionSections)
        {
            otherVersionSection.Update(nextVersionSectionId: newMainSection.Id);
            var oldContributor = await session.Query<PaperContributorEntity>()
                                     .Where(x => x.PaperId == otherVersionSection.PaperId &&
                                                 x.SectionId == otherVersionSection.Id)
                                     .FirstOrDefaultAsync(cancellationToken)
                                 ?? throw new NotFoundException(MessageCode.ContributorIsNotExists,
                                     otherVersionSection.Id);

            // Mark old contributor to old main section
            oldContributor.Update(markSectionId: section.Id);

            // Clone contributor to new record with new main section
            var newContributor = PaperContributorEntity.Create(
                id: Guid.NewGuid(),
                sectionRole: oldContributor.SectionRole,
                paperId: oldContributor.PaperId,
                sectionId: newMainSection.Id,
                memberId: oldContributor.MemberId,
                markSectionId: newMainSection.Id // Mark new contributor to new main section
            );
            await UpsertReferenceSectionAsync(otherVersionSection.PaperId, oldContributor.MemberId,
                newMainSection.References,
                otherVersionSection.CreatedBy, cancellationToken);
            session.Update(oldContributor);
            session.Store(newContributor);
        }

        var versionSectionIds = otherVersionSections
            .Select(x => x.Id)
            .Append(oldMainSection.Id)
            .ToHashSet();

        var mainSectionReferenceIds = (section.References ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToHashSet();

        var paper = await session.LoadAsync<PaperEntity>(section.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, section.PaperId.ToString());

        var references = (paper.References ?? [])
            .Select(reference => new PaperReference
            {
                PaperId = reference.PaperId,
                SectionIds = reference.SectionIds
                    .Where(sectionId => !versionSectionIds.Contains(sectionId))
                    .Distinct()
                    .ToList()
            })
            .Where(reference => reference.SectionIds.Count > 0)
            .ToList();

        foreach (var paperId in mainSectionReferenceIds)
        {
            var reference = references.FirstOrDefault(x => x.PaperId == paperId);

            if (reference is null)
            {
                references.Add(new PaperReference
                {
                    PaperId = paperId,
                    SectionIds = [newMainSection.Id]
                });
                continue;
            }

            if (!reference.SectionIds.Contains(newMainSection.Id))
                reference.SectionIds.Add(newMainSection.Id);
        }

        paper.Update(references: references);
        session.Update(paper);

        session.Store(newMainSection);
        session.Update(section);
        session.Update(oldMainSection);

        await session.SaveChangesAsync(cancellationToken);

        var referenceMainSection = await session.Query<SectionEntity>()
                                       .Where(s => s.PaperId == section.PaperId
                                                   && (s.Title!.EqualsIgnoreCase(SectionConstants.ReferencesTitle) ||
                                                       s.Title!.EqualsIgnoreCase(SectionConstants.ReferenceTitle))
                                                   && s.IsMainSection == true)
                                       .FirstOrDefaultAsync(cancellationToken)
                                   ?? throw new NotFoundException(MessageCode.SectionIsNotExists);

        var (distinctReferences, referenceContent) =
            await BuildReferenceContentAsync(referenceMainSection, cancellationToken);

        referenceMainSection.Update(references: distinctReferences, content: referenceContent);
        session.Update(referenceMainSection);

        await session.SaveChangesAsync(cancellationToken);

        return newMainSection.Id;
    }

    private async Task<(List<Guid>, string)> BuildReferenceContentAsync(
        SectionEntity referenceSection,
        CancellationToken cancellationToken)
    {
        var mainSections = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == referenceSection.PaperId &&
                        x.Id != referenceSection.Id &&
                        x.IsMainSection == true)
            .ToListAsync(cancellationToken);

        var distinctReferences = mainSections
            .Where(x => x.References != null)
            .SelectMany(x => x.References!)
            .Distinct()
            .ToList();

        if (distinctReferences.Count == 0)
            return ([], string.Empty);

        var items = await session.Query<PaperBankEntity>()
            .Where(x => distinctReferences.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var combinedReferenceContent = string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            items
                .Select(x => x.ReferenceContent)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct());

        return (distinctReferences, string.IsNullOrWhiteSpace(combinedReferenceContent)
            ? string.Empty
            : $"\\begin{{filecontents*}}{{references.bib}}{Environment.NewLine}{Environment.NewLine}" +
              combinedReferenceContent +
              $"{Environment.NewLine}{Environment.NewLine}\\end{{filecontents*}}{Environment.NewLine}{Environment.NewLine}" +
              "\\addbibresource{references.bib}");
    }

    private async Task UpsertReferenceSectionAsync(
        Guid paperId,
        Guid memberId,
        List<Guid>? mainSectionReferences,
        string? createdBy,
        CancellationToken cancellationToken)
    {
        var mainSectionIds = await session.Query<SectionEntity>()
            .Where(x => x.PaperId == paperId && x.IsMainSection == true)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (mainSectionIds.Count == 0) return;

        var memberContributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == paperId &&
                        x.MemberId == memberId &&
                        mainSectionIds.Contains(x.MarkSectionId))
            .ToListAsync(cancellationToken);

        if (memberContributors.Count == 0) return;

        var assignedSectionIds = memberContributors
            .Where(x => x.SectionId != null)
            .Select(x => x.SectionId!.Value)
            .Distinct()
            .ToList();

        var assignedSections = assignedSectionIds.Count == 0
            ? []
            : await session.Query<SectionEntity>()
                .Where(x => assignedSectionIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var sectionMap = assignedSections.ToDictionary(x => x.Id, x => x);

        // Build references only from sections assigned to this member, excluding reference sections.
        var nonReferenceAssignedSections = assignedSections
            .Where(x => !x.Title!.ToLower().Contains("reference"))
            .ToList();

        var distinctReferenceIds = nonReferenceAssignedSections
            .Where(x => x.References != null)
            .SelectMany(x => x.References!)
            .Concat(mainSectionReferences ?? [])
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        var paperBanks = distinctReferenceIds.Count == 0
            ? []
            : await session.Query<PaperBankEntity>()
                .Where(x => distinctReferenceIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var paperBankContentMap = paperBanks.ToDictionary(x => x.Id, x => x.ReferenceContent);
        var generatedReferenceContent = BuildReferenceSectionContent(
            distinctReferenceIds
                .Where(paperBankContentMap.ContainsKey)
                .Select(id => paperBankContentMap[id]));

        var mainReferenceSection = assignedSections.FirstOrDefault(x =>
                                       x.IsMainSection == true &&
                                       SectionConstants.IsReferenceSection(x.Title))
                                   ?? await session.Query<SectionEntity>()
                                       .Where(x => x.PaperId == paperId &&
                                                   x.IsMainSection == true &&
                                                   (x.Title!.EqualsIgnoreCase(SectionConstants.ReferencesTitle) ||
                                                    x.Title!.EqualsIgnoreCase(SectionConstants.ReferenceTitle)))
                                       .FirstOrDefaultAsync(cancellationToken);

        if (mainReferenceSection == null) return;

        var referenceContributor = memberContributors.FirstOrDefault(x =>
            x.SectionId != null &&
            sectionMap.TryGetValue(x.SectionId.Value, out var memberSection) &&
            SectionConstants.IsReferenceSection(memberSection.Title));

        referenceContributor ??= memberContributors
            .FirstOrDefault(x => x.MarkSectionId == mainReferenceSection.Id);

        SectionEntity? currentReferenceSection = null;
        if (referenceContributor?.SectionId is { } currentReferenceSectionId)
            sectionMap.TryGetValue(currentReferenceSectionId, out currentReferenceSection);

        if (currentReferenceSection != null && currentReferenceSection.IsMainSection != true)
        {
            currentReferenceSection.Update(content: generatedReferenceContent, references: distinctReferenceIds);
            session.Update(currentReferenceSection);
            return;
        }

        var newRefSection = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: generatedReferenceContent,
            paperId: mainReferenceSection.PaperId,
            displayOrder: mainReferenceSection.DisplayOrder,
            numbered: mainReferenceSection.Numbered,
            isMainSection: false,
            isOldMainSection: false,
            version: mainReferenceSection.Version,
            title: mainReferenceSection.Title,
            sectionSumary: mainReferenceSection.SectionSumary,
            description: mainReferenceSection.Description,
            rule: mainReferenceSection.Rule,
            parentSectionId: mainReferenceSection.ParentSectionId,
            previousVersionSectionId: mainReferenceSection.Id,
            references: distinctReferenceIds,
            createdBy: createdBy,
            paperRule: mainReferenceSection.PaperRule,
            projectRule: mainReferenceSection.ProjectRule,
            sectionRule: mainReferenceSection.SectionRule,
            packages: mainReferenceSection.Packages
        );
        session.Store(newRefSection);

        if (referenceContributor != null)
        {
            referenceContributor.Update(sectionId: newRefSection.Id, markSectionId: mainReferenceSection.Id);
            session.Update(referenceContributor);
        }
    }

    private static string BuildReferenceSectionContent(IEnumerable<string?> referenceEntries)
    {
        var combinedReferenceContent = string.Join(
            $"{Environment.NewLine}{Environment.NewLine}",
            referenceEntries
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x!.Trim())
                .Distinct());

        if (string.IsNullOrWhiteSpace(combinedReferenceContent))
            return string.Empty;

        return $"\\begin{{filecontents*}}{{references.bib}}{Environment.NewLine}" +
               combinedReferenceContent +
               $"{Environment.NewLine}\\end{{filecontents*}}{Environment.NewLine}" +
               "\\addbibresource{references.bib}";
    }
}
using JasperFx.Core;
using Lab.Application.Dtos.Sections;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;
using PaperReference = Lab.Domain.Models.Reference;

namespace Lab.Application.Features.Section.Commands.UpdateReference;

public record UpdateReferenceCommand(UpdateReferenceDto Dto, Guid UserId, string UserName, Guid Id) : ICommand<Guid>;

public class UpdateReferenceCommandValidator : AbstractValidator<UpdateReferenceCommand>
{
    public UpdateReferenceCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.PaperId)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.PaperIdIsRequired);
                // RuleFor(x => x.Dto.Content)
                //     .NotEmpty()
                //     .WithMessage(MessageCode.SectionContentIsRequired)
                //     .NotNull()
                //     .WithMessage(MessageCode.SectionContentIsRequired);
                RuleFor(x => x.Dto.PaperBankIds)
                    .NotNull()
                    .Must(ids => ids != null && ids.All(id => id != Guid.Empty))
                    .WithMessage(MessageCode.PaperBankIdsIsRequired);
                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage(MessageCode.SectionIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.SectionIdIsRequired);
            });
    }
}

public class UpdateReferenceCommandHandler(IDocumentSession session, IManagementApiService service)
    : ICommandHandler<UpdateReferenceCommand, Guid>
{
    public async Task<Guid> Handle(UpdateReferenceCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        Guid responseId;

        var memberInfo = await service.GetMemberByPaperIdAsync(dto.PaperId, request.UserId, cancellationToken);
        if (memberInfo == null)
            throw new UnauthorizedException(MessageCode.AccessDenied);

        var (_, memberId) = memberInfo.Value;

        await session.BeginTransactionAsync(cancellationToken);

        var currentEditSection = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                                 ?? throw new NotFoundException(MessageCode.SectionIsNotExists);

        var effectiveSectionId = currentEditSection.Id;

        var contributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == currentEditSection.PaperId &&
                        x.MemberId == memberId &&
                        x.SectionId == currentEditSection.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (contributor == null ||
            AuthorizeConstants.SectionRead.EqualsIgnoreCase(contributor.SectionRole) ||
            contributor.SectionRole.IsNullOrEmpty())
            throw new UnauthorizedException(MessageCode.AccessDenied);

        var referenceMainSection = await session.Query<SectionEntity>()
                                       .Where(s => s.PaperId == dto.PaperId
                                                   && (s.Title!.EqualsIgnoreCase(SectionConstants.ReferencesTitle) ||
                                                       s.Title!.EqualsIgnoreCase(SectionConstants.ReferenceTitle))
                                                   && s.IsMainSection == true)
                                       .FirstOrDefaultAsync(cancellationToken)
                                   ?? throw new NotFoundException(MessageCode.SectionIsNotExists);

        var currentUserContributor = await session.Query<PaperContributorEntity>()
                                         .Where(x => x.PaperId == dto.PaperId &&
                                                     x.MemberId == memberId &&
                                                     x.MarkSectionId == referenceMainSection.Id)
                                         .FirstOrDefaultAsync(cancellationToken)
                                     ?? throw new UnauthorizedException(MessageCode.AccessDenied);

        var currentReferenceSection =
            await session.LoadAsync<SectionEntity>(currentUserContributor.SectionId!, cancellationToken)
            ?? throw new NotFoundException(MessageCode.SectionIsNotExists);

        var selectedPaperIds = dto.PaperBankIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        var selectedPaperIdSet = selectedPaperIds.ToHashSet();

        var contributorSectionIds = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == dto.PaperId &&
                        x.MemberId == memberId &&
                        x.SectionId != null &&
                        x.MarkSectionId != referenceMainSection.Id)
            .Select(x => x.SectionId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var mergedReferenceIds = new List<Guid>();
        var mergedReferenceIdSet = new HashSet<Guid>();

        foreach (var paperId in selectedPaperIds)
        {
            if (mergedReferenceIdSet.Add(paperId))
                mergedReferenceIds.Add(paperId);
        }

        var otherSectionIds = contributorSectionIds
            .Where(x => x != effectiveSectionId)
            .ToList();

        if (otherSectionIds.Count > 0)
        {
            var otherSections = await session.Query<SectionEntity>()
                .Where(x => otherSectionIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            foreach (var section in otherSections)
            {
                if (section.References == null) continue;

                foreach (var referenceId in section.References.Where(x => x != Guid.Empty))
                {
                    if (mergedReferenceIdSet.Add(referenceId))
                        mergedReferenceIds.Add(referenceId);
                }
            }
        }

        var referenceSectionPaperBankIds = mergedReferenceIds;

        var paperBanks = referenceSectionPaperBankIds.Count == 0
            ? new List<PaperBankEntity>()
            : await session.Query<PaperBankEntity>()
                .Where(x => referenceSectionPaperBankIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

        var paperBankContentMap = paperBanks.ToDictionary(x => x.Id, x => x.ReferenceContent);
        var generatedReferenceContent = BuildReferenceSectionContent(
            referenceSectionPaperBankIds
                .Where(paperBankContentMap.ContainsKey)
                .Select(id => paperBankContentMap[id]));

        if (currentReferenceSection.IsMainSection == true)
        {
            var newReferenceSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: generatedReferenceContent,
                paperId: referenceMainSection.PaperId,
                displayOrder: referenceMainSection.DisplayOrder,
                numbered: referenceMainSection.Numbered,
                isMainSection: null,
                isOldMainSection: null,
                title: referenceMainSection.Title,
                sectionSumary: referenceMainSection.SectionSumary,
                description: referenceMainSection.Description,
                rule: referenceMainSection.Rule,
                parentSectionId: referenceMainSection.ParentSectionId,
                previousVersionSectionId: referenceMainSection.Id,
                references: referenceSectionPaperBankIds,
                createdBy: request.UserName);

            currentUserContributor.Update(sectionId: newReferenceSection.Id, markSectionId: referenceMainSection.Id);
            session.Store(newReferenceSection);
            session.Update(currentUserContributor);
        }
        else
        {
            currentReferenceSection.Update(content: generatedReferenceContent, references: referenceSectionPaperBankIds);
            session.Update(currentReferenceSection);
        }

        if (currentEditSection.IsMainSection is null or false)
        {
            currentEditSection.Update(references: selectedPaperIds);
            session.Update(currentEditSection);
            responseId = currentEditSection.Id;
        }
        else
        {
            var newEditSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: currentEditSection.Content,
                paperId: currentEditSection.PaperId,
                displayOrder: currentEditSection.DisplayOrder,
                numbered: currentEditSection.Numbered,
                isMainSection: null,
                isOldMainSection: null,
                title: currentEditSection.Title,
                sectionSumary: currentEditSection.SectionSumary,
                description: currentEditSection.Description,
                rule: currentEditSection.Rule,
                parentSectionId: currentEditSection.ParentSectionId,
                previousVersionSectionId: currentEditSection.Id,
                references: selectedPaperIds,
                createdBy: request.UserName);

            contributor.Update(sectionId: newEditSection.Id, markSectionId: currentEditSection.Id);
            session.Store(newEditSection);
            session.Update(contributor);
            responseId = newEditSection.Id;
        }

        var paper = await session.LoadAsync<PaperEntity>(dto.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, dto.PaperId.ToString());


        List<PaperReference> references = paper.References ?? new List<PaperReference>();

        for (var i = references.Count - 1; i >= 0; i--)
        {
            var reference = references[i];

            if (reference.SectionIds.Contains(effectiveSectionId) && !selectedPaperIdSet.Contains(reference.PaperId))
            {
                reference.SectionIds.RemoveAll(x => x == effectiveSectionId);
                if (reference.SectionIds.Count == 0)
                    references.RemoveAt(i);
            }
        }

        foreach (var paperId in selectedPaperIdSet)
        {
            var reference = references.FirstOrDefault(x => x.PaperId == paperId);

            if (reference is null)
            {
                references.Add(new PaperReference
                {
                    PaperId = paperId,
                    SectionIds = new List<Guid> { effectiveSectionId }
                });

                continue;
            }

            if (!reference.SectionIds.Contains(effectiveSectionId))
                reference.SectionIds.Add(effectiveSectionId);
        }

        paper.Update(references: references);
        session.Update(paper);

        await session.SaveChangesAsync(cancellationToken);

        return responseId;
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

        return $"\\begin{{filecontents}}{{references.bib}}{Environment.NewLine}" +
               combinedReferenceContent +
               $"{Environment.NewLine}\\end{{filecontents}}{Environment.NewLine}" +
               "\\addbibresource{references.bib}" +
               $"{Environment.NewLine}\\printbibliography";
    }
}
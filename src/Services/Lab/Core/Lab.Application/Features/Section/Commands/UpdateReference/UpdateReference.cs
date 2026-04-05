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
                RuleFor(x => x.Dto.Content)
                    .NotEmpty()
                    .WithMessage(MessageCode.SectionContentIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.SectionContentIsRequired);
                RuleFor(x => x.Dto.PaperBankIds)
                    .NotNull()
                    .Must(ids => ids != null && ids.Any(id => id != Guid.Empty))
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

        if (currentReferenceSection.IsMainSection == true)
        {
            var newReferenceSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: dto.Content,
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
                references: dto.PaperBankIds,
                createdBy: request.UserName);

            currentUserContributor.Update(sectionId: newReferenceSection.Id, markSectionId: referenceMainSection.Id);
            session.Store(newReferenceSection);
            session.Update(currentUserContributor);
        }
        else
        {
            currentReferenceSection.Update(content: dto.Content, references: dto.PaperBankIds);
            session.Update(currentReferenceSection);
        }

        currentEditSection.Update(references: dto.PaperBankIds);
        session.Update(currentEditSection);

        var paper = await session.LoadAsync<PaperEntity>(dto.PaperId, cancellationToken)
                    ?? throw new NotFoundException(MessageCode.PaperIsNotExists, dto.PaperId.ToString());

        var selectedPaperIds = dto.PaperBankIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToHashSet();

        List<PaperReference> references = paper.References ?? new List<PaperReference>();

        for (var i = references.Count - 1; i >= 0; i--)
        {
            var reference = references[i];

            if (reference.SectionIds.Contains(effectiveSectionId) && !selectedPaperIds.Contains(reference.PaperId))
            {
                reference.SectionIds.RemoveAll(x => x == effectiveSectionId);
                if (reference.SectionIds.Count == 0)
                    references.RemoveAt(i);
            }
        }

        foreach (var paperId in selectedPaperIds)
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

        return effectiveSectionId;
    }
}
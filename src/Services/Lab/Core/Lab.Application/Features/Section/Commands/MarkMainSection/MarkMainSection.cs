using JasperFx.Core;
using Lab.Application.Dtos.Sections;
using Lab.Application.Rules;
using Lab.Application.Services;
using Lab.Domain.Entities;
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

        // Clone main section to new record
        var newMainSection = SectionEntity.Create(
            id: Guid.NewGuid(),
            content: section.Content,
            paperId: section.PaperId,
            displayOrder: section.DisplayOrder,
            numbered: section.Numbered,
            isMainSection: true,
            isOldMainSection: false,
            title: section.Title,
            sectionSumary: section.SectionSumary,
            description: section.Description,
            rule: section.Rule,
            parentSectionId: section.ParentSectionId,
            previousVersionSectionId: section.Id,
            references: section.References,
            createdBy: request.UserName,
            paperRule: section.PaperRule,
            projectRule: section.ProjectRule,
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
            session.Update(oldContributor);
            session.Store(newContributor);
        }

        session.Store(newMainSection);
        session.Update(section);
        session.Update(oldMainSection);

        await session.SaveChangesAsync(cancellationToken);

        return newMainSection.Id;
    }
}
using Lab.Application.Dtos.Sections;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Section.Commands.UpsertSection;

public record UpsertSectionCommand(UpsertSectionDto Dto, Guid Id) : ICommand<Guid>;

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
    public async Task<Guid> Handle(UpsertSectionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, request.Id);

        var query = session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == section.PaperId && x.MemberId == dto.MemberId && x.SectionId == section.Id);
        var contributor = await query.FirstOrDefaultAsync(cancellationToken);
        if (contributor == null || contributor.SectionRole == AuthorizeConstants.SectionRead)
            throw new UnauthorizedException(MessageCode.AccessDenied);


        //If writer is author the section will update in main section
        var isAuthor = string.Equals(contributor.SectionRole, AuthorizeConstants.PaperAuthor,
            StringComparison.OrdinalIgnoreCase);
        if (isAuthor)
        {
            // Author can update the main section directly
            section.Update(
                content: dto.Content,
                numbered: dto.Numbered,
                title: dto.Title,
                sectionSumary: dto.SectionSumary,
                parentSectionId: dto.ParentSectionId
            );

            session.Update(section);
            await session.SaveChangesAsync(cancellationToken);
            return section.Id;
        }

        // If writer is not author, the section will be created as a new version of main section, and the new section will be updated by writer, the main section will be updated by author
        if (section.IsMainSection == true)
        {
            var newSection = SectionEntity.Create(
                id: Guid.NewGuid(),
                content: dto.Content,
                paperId: section.PaperId,
                displayOrder: section.DisplayOrder,
                numbered: dto.Numbered,
                isMainSection: false,
                title: dto.Title,
                sectionSumary: dto.SectionSumary,
                parentSectionId: dto.ParentSectionId,
                //Mark new section as new version of main section
                previousVersionSectionId: section.Id
            );

            // Update contributor to point to new section
            contributor.Update(
                sectionId: newSection.Id,
                markSectionId: section.Id
            );

            session.Store(newSection);
            session.Update(contributor);
            await session.SaveChangesAsync(cancellationToken);
            return newSection.Id;
        }

        // If the section is not main section, it means the section is already a new version of main section, so just update the section directly
        section.Update(
            content: dto.Content,
            numbered: dto.Numbered,
            title: dto.Title,
            sectionSumary: dto.SectionSumary,
            parentSectionId: dto.ParentSectionId
        );

        session.Update(section);
        await session.SaveChangesAsync(cancellationToken);
        return section.Id;
    }
}
using Lab.Application.Dtos.Sections;
using Lab.Application.Services;
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
                RuleFor(x => x.Dto.ProjectId)
                    .NotEmpty()
                    .WithMessage(MessageCode.ProjectIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.ProjectIdIsRequired);

                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage(MessageCode.SectionIdIsRequired)
                    .NotNull()
                    .WithMessage(MessageCode.SectionIdIsRequired);
            });
    }
}

public class UpsertSectionCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : ICommandHandler<UpsertSectionCommand, Guid>
{
    public async Task<Guid> Handle(UpsertSectionCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new ClientValidationException(MessageCode.SectionIdIsRequired, request.Id);

        //TODO: Handle check paper contributor, if not throw forbidden exception

        //If writer is author the section will update in main section
        var role = await managementApiService.GetMyProjectRoleAsync(dto.ProjectId, cancellationToken);
        var isAuthor = string.Equals(role, AuthorizeConstants.ProjectAuthor, StringComparison.OrdinalIgnoreCase);
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

        //If the section is main section, create a new section and mark it as new version of main section, and update main section to be not main section, and return new section id, otherwise we will update the section and return the same section id
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

            session.Store(newSection);
            await session.SaveChangesAsync(cancellationToken);
            return newSection.Id;
        }

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
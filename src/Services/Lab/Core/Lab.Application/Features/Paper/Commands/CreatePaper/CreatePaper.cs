using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Paper.Commands.CreatePaper;

public record CreatePaperCommand(CreatePaperDto Dto) : ICommand<Guid>;

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
            });
    }
}

public class CreatePaperCommandHandler(
    IDocumentSession session,
    IManagementApiService managementApiService) : ICommandHandler<CreatePaperCommand, Guid>
{
    public async Task<Guid> Handle(CreatePaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var entity = PaperEntity.Create(
            id: Guid.NewGuid(),
            title: dto.Title,
            template: dto.Template,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status ?? PaperStatus.Processing,
            paperType: dto.PaperType
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
            foreach (var template in dto.Sections)
            {
                var section = SectionEntity.Create(
                    id: template.Id,
                    content: template.Content,
                    paperId: entity.Id,
                    displayOrder: template.DisplayOrder,
                    numbered: template.Numbered,
                    isMainSection: true,
                    title: template.Title,
                    sectionSumary: template.SectionSumary,
                    parentSectionId: template.ParentSectionId
                );
                session.Store(section);
            }

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        if (dto.ProjectId != Guid.Empty)
        {
            await managementApiService.CreateSubProjectAsync(
                dto.ProjectId, entity.Id, dto.Title, cancellationToken);
        }

        return entity.Id;
    }
}
using Lab.Application.Dtos.Papers;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.Paper.Commands.InitPaper;

public record InitPaperCommand(InitPaperDto Dto) : ICommand<Guid>;

public class InitPaperCommandValidator : AbstractValidator<InitPaperCommand>
{
    public InitPaperCommandValidator()
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

public class InitPaperCommandHandler(IDocumentSession session) : ICommandHandler<InitPaperCommand, Guid>
{
    public async Task<Guid> Handle(InitPaperCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        await session.BeginTransactionAsync(cancellationToken);

        var entity = PaperEntity.Init(
            id: Guid.NewGuid(),
            title: dto.Title,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status ?? PaperStatus.Draft,
            paperType: dto.PaperType
        );

        if (dto.Sections != null && dto.Sections.Count != 0)
            foreach (var template in dto.Sections)
            {
                var section = SectionEntity.Create(
                    id: Guid.NewGuid(),
                    content: template.Content,
                    paperId: entity.Id,
                    displayOrder: template.DisplayOrder,
                    title: template.Title,
                    sectionSumary: template.SectionSumary,
                    parentSectionId: template.ParentSectionId
                );
                session.Store(section);
            }

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
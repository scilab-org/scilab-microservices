using Lab.Application.Dtos.PaperContributors;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;

public sealed record CreatePaperContributorCommand(CreatePaperContributorDto Dto) : ICommand<Guid>;

public sealed class CreatePaperContributorCommandValidator : AbstractValidator<CreatePaperContributorCommand>
{
    #region Ctors

    public CreatePaperContributorCommandValidator()
    {
        RuleFor(x => x.Dto.SectionRole).NotNull()
            .WithMessage(MessageCode.SectionRoleIsRequired);
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.PaperId)
                    .NotEmpty()
                    .WithMessage(MessageCode.PaperIdIsRequired);

                RuleFor(x => x.Dto.MemberId)
                    .NotEmpty()
                    .WithMessage(MessageCode.MemberIdIsRequired);
            });
    }

    #endregion
}

public sealed class CreatePaperContributorCommandHandler(IDocumentSession session) : ICommandHandler<CreatePaperContributorCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreatePaperContributorCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var mainEntity  = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: dto.SectionRole!,
            paperId: dto.PaperId,
            sectionId: dto.SectionId,
            memberId: dto.MemberId,
            markSectionId: dto.MarkSectionId);

        session.Store(mainEntity );
        
        // Check the "reference" section and assign if not already assigned
        var candidateSections = await session.Query<SectionEntity>()
            .Where(s => s.PaperId == dto.PaperId && s.IsMainSection == true && s.IsOldMainSection == false)
            .ToListAsync(cancellationToken);

        var referenceSection = candidateSections
            .FirstOrDefault(s => SectionConstants.IsReferenceSection(s.Title));
        
        if (referenceSection != null)
        {
            var alreadyAssigned = await session.Query<PaperContributorEntity>()
                .AnyAsync(pc =>
                        pc.PaperId == dto.PaperId &&
                        pc.MemberId == dto.MemberId &&
                        (pc.SectionId == referenceSection.Id || pc.MarkSectionId == referenceSection.Id),
                    cancellationToken);

            if (!alreadyAssigned)
            {
                var referenceEntity = PaperContributorEntity.Create(
                    id: Guid.NewGuid(),
                    sectionRole: dto.SectionRole!,
                    paperId: dto.PaperId,
                    sectionId: referenceSection.Id,
                    memberId: dto.MemberId,
                    markSectionId: referenceSection.Id
                );

                session.Store(referenceEntity);
            }
        }
        
        await session.SaveChangesAsync(cancellationToken);

        return mainEntity.Id;
    }

    #endregion
}
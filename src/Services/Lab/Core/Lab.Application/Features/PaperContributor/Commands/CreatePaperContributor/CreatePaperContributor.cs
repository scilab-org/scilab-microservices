using Lab.Application.Dtos.PaperContributors;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.PaperContributor.Commands.CreatePaperContributor;

public sealed record CreatePaperContributorCommand(CreatePaperContributorDto Dto) : ICommand<List<Guid>>;

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

                RuleFor(x => x.Dto.MemberIds)
                    .NotEmpty()
                    .WithMessage(MessageCode.MemberIdIsRequired);
            });
    }

    #endregion
}

public sealed class CreatePaperContributorCommandHandler(IDocumentSession session) : ICommandHandler<CreatePaperContributorCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(CreatePaperContributorCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;
        var createdIds = new List<Guid>();

        // Check the "reference" section once for the paper (only needed for non-PaperAuthor roles)
        IReadOnlyList<SectionEntity>? candidateSections = null;
        SectionEntity? referenceSection = null;

        if (dto.SectionRole != AuthorizeConstants.PaperAuthor)
        {
            candidateSections = await session.Query<SectionEntity>()
                .Where(s => s.PaperId == dto.PaperId && s.IsMainSection == true && s.IsOldMainSection == false)
                .ToListAsync(cancellationToken);

            referenceSection = candidateSections
                .FirstOrDefault(s => SectionConstants.IsReferenceSection(s.Title));
        }

        foreach (var memberId in dto.MemberIds)
        {
            var mainEntity = PaperContributorEntity.Create(
                id: Guid.NewGuid(),
                sectionRole: dto.SectionRole!,
                paperId: dto.PaperId,
                sectionId: dto.SectionId,
                memberId: memberId,
                markSectionId: dto.MarkSectionId);

            session.Store(mainEntity);
            createdIds.Add(mainEntity.Id);

            if (referenceSection != null)
            {
                var alreadyAssigned = await session.Query<PaperContributorEntity>()
                    .AnyAsync(pc =>
                            pc.PaperId == dto.PaperId &&
                            pc.MemberId == memberId &&
                            (pc.SectionId == referenceSection.Id || pc.MarkSectionId == referenceSection.Id),
                        cancellationToken);

                if (!alreadyAssigned)
                {
                    var referenceEntity = PaperContributorEntity.Create(
                        id: Guid.NewGuid(),
                        sectionRole: dto.SectionRole!,
                        paperId: dto.PaperId,
                        sectionId: referenceSection.Id,
                        memberId: memberId,
                        markSectionId: referenceSection.Id
                    );

                    session.Store(referenceEntity);
                }
            }
        }

        await session.SaveChangesAsync(cancellationToken);

        return createdIds;
    }

    #endregion
}
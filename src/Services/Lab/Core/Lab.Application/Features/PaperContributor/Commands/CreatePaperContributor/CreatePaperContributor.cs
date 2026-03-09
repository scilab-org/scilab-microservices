using Lab.Application.Dtos.PaperContributors;
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

        var entity = PaperContributorEntity.Create(
            id: Guid.NewGuid(),
            sectionRole: dto.SectionRole!,
            paperId: dto.PaperId,
            sectionId: dto.SectionId,
            memberId: dto.MemberId,
            markSectionId: dto.MarkSectionId);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
}
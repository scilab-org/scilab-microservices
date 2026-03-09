using Lab.Application.Dtos.PaperContributors;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.PaperContributor.Commands.UpdatePaperContributor;

public sealed record UpdatePaperContributorCommand(Guid Id, UpdatePaperContributorDto Dto) : ICommand<Guid>;

public sealed class UpdatePaperContributorCommandValidator : AbstractValidator<UpdatePaperContributorCommand>
{
    #region Ctors

    public UpdatePaperContributorCommandValidator()
    {
        RuleFor(x => x.Id).NotNull()
            .WithMessage(MessageCode.PaperContributorIdIsRequired);
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.MemberId)
                    .NotEmpty()
                    .WithMessage(MessageCode.MemberIdIsRequired);
            });
    }

    #endregion
}

public class UpdatePaperContributorCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdatePaperContributorCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdatePaperContributorCommand command, CancellationToken cancellationToken)
    {
        var current = await session.LoadAsync<PaperContributorEntity>(command.Id, cancellationToken);
        if (current == null)
            throw new ("Template not found.");
        
        current.Update(
            sectionRole: command.Dto.SectionRole ?? current.SectionRole,
            memberId: command.Dto.MemberId ?? current.MemberId,
            sectionId: command.Dto.SectionId ?? current.SectionId,
            markSectionId: command.Dto.MarkSectionId ?? current.MarkSectionId
        );
        session.Store(current);
        
        await session.SaveChangesAsync(cancellationToken);

        return current.Id;
        
    }

    #endregion
}

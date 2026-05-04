using JasperFx.Core;
using Lab.Application.Dtos.CheckLists;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.CheckList.Commands.UpdateCheckList;

public record UpdateCheckListCommand(UpdateCheckListDto Dto, Guid Id, string UserName) : ICommand<Guid>;

public class UpdateCheckListCommandValidator : AbstractValidator<UpdateCheckListCommand>
{
    public UpdateCheckListCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage(MessageCode.CheckListIdIsRequired);

                RuleFor(x => x.Dto.Section)
                    .NotEmpty().WithMessage(MessageCode.CheckListSectionIsRequired)
                    .NotNull().WithMessage(MessageCode.CheckListSectionIsRequired);

                RuleFor(x => x.Dto.RuleName)
                    .NotEmpty().WithMessage(MessageCode.CheckListRuleNameIsRequired)
                    .NotNull().WithMessage(MessageCode.CheckListRuleNameIsRequired);

                RuleFor(x => x.Dto.Item)
                    .NotEmpty().WithMessage(MessageCode.CheckListItemIsRequired)
                    .NotNull().WithMessage(MessageCode.CheckListItemIsRequired);

                RuleFor(x => x.Dto.Weight)
                    .GreaterThan(0).WithMessage(MessageCode.CheckListWeightIsRequired);
            });
    }
}

public class UpdateCheckListCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateCheckListCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCheckListCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<CheckListEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.CheckListIsNotExists, request.Id);

        entity.Update(
            section: request.Dto.Section.Trim(),
            ruleName: request.Dto.RuleName.Trim(),
            item: request.Dto.Item.Trim(),
            weight: request.Dto.Weight,
            modifiedBy: request.UserName);

        session.Update(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
using Lab.Application.Dtos.CheckLists;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.CheckList.Commands.CreateCheckList;

public record CreateCheckListCommand(CreateCheckListDto Dto, string UserName) : ICommand<Guid>;

public class CreateCheckListCommandValidator : AbstractValidator<CreateCheckListCommand>
{
    public CreateCheckListCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
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

public class CreateCheckListCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateCheckListCommand, Guid>
{
    public async Task<Guid> Handle(CreateCheckListCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = CheckListEntity.Create(
            id: Guid.NewGuid(),
            section: request.Dto.Section.Trim(),
            ruleName: request.Dto.RuleName.Trim(),
            item: request.Dto.Item.Trim(),
            weight: request.Dto.Weight,
            createBy: request.UserName);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
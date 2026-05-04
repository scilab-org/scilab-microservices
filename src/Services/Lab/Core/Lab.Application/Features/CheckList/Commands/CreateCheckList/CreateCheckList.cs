using Lab.Application.Dtos.CheckLists;
using Lab.Domain.Entities;
using Lab.Domain.Models;
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

                RuleFor(x => x.Dto.Items)
                    .NotNull().WithMessage(MessageCode.CheckListItemIsRequired)
                    .NotEmpty().WithMessage(MessageCode.CheckListItemIsRequired);

                RuleForEach(x => x.Dto.Items).ChildRules(item =>
                {
                    item.RuleFor(i => i.Name)
                        .NotEmpty().WithMessage(MessageCode.CheckListRuleNameIsRequired)
                        .NotNull().WithMessage(MessageCode.CheckListRuleNameIsRequired);

                    item.RuleFor(i => i.Rule)
                        .NotEmpty().WithMessage(MessageCode.CheckListItemIsRequired)
                        .NotNull().WithMessage(MessageCode.CheckListItemIsRequired);

                    item.RuleFor(i => i.Weight)
                        .GreaterThan(0).WithMessage(MessageCode.CheckListWeightIsRequired);
                });
            });
    }
}

public class CreateCheckListCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateCheckListCommand, Guid>
{
    public async Task<Guid> Handle(CreateCheckListCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var dupplicateSection = await session.Query<CheckListEntity>()
            .Where(x => x.Section.ToLower() == request.Dto.Section.ToLower().Trim())
            .AnyAsync(cancellationToken);

        if (dupplicateSection)
        {
            throw new ValidationException(MessageCode.CheckListSectionAlreadyExists);
        }

        var entity = CheckListEntity.Create(
            id: Guid.NewGuid(),
            section: request.Dto.Section.Trim(),
            items: request.Dto.Items.Select(x => new Item
            {
                Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                Name = x.Name.Trim(),
                Rule = x.Rule.Trim(),
                Weight = x.Weight
            }).ToList(),
            createBy: request.UserName);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
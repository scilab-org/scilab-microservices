using JasperFx.Core;
using Lab.Application.Dtos.CheckLists;
using Lab.Domain.Entities;
using Lab.Domain.Models;
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

public class UpdateCheckListCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateCheckListCommand, Guid>
{
    public async Task<Guid> Handle(UpdateCheckListCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<CheckListEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.CheckListIsNotExists, request.Id);

        var dupplicateSection = await session.Query<CheckListEntity>()
            .Where(x => x.Id != request.Id && x.Section.ToLower() == request.Dto.Section.ToLower().Trim())
            .AnyAsync(cancellationToken);

        if (dupplicateSection)
        {
            throw new ValidationException(MessageCode.CheckListSectionAlreadyExists);
        }

        entity.Update(
            section: request.Dto.Section.Trim(),
            items: request.Dto.Items.Select(x => new Item
            {
                Id = x.Id == Guid.Empty ? Guid.NewGuid() : x.Id,
                Name = x.Name.Trim(),
                Rule = x.Rule.Trim(),
                Weight = x.Weight
            }).ToList(),
            modifiedBy: request.UserName);

        session.Update(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
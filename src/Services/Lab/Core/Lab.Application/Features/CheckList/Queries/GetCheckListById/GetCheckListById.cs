using AutoMapper;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.CheckList.Queries.GetCheckListById;

public record GetCheckListByIdQuery(Guid Id) : ICommand<GetCheckListByIdResult>;

public class GetCheckListByIdQueryValidator : AbstractValidator<GetCheckListByIdQuery>
{
    public GetCheckListByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage(MessageCode.CheckListIdIsRequired)
            .NotEmpty()
            .WithMessage(MessageCode.CheckListIdIsRequired);
    }
}

public class GetCheckListByIdQueryHandler(
    IDocumentSession session,
    IMapper mapper)
    : ICommandHandler<GetCheckListByIdQuery, GetCheckListByIdResult>
{
    public async Task<GetCheckListByIdResult> Handle(GetCheckListByIdQuery request, CancellationToken cancellationToken)
    {
        var checkList = await session.LoadAsync<CheckListEntity>(request.Id, cancellationToken);

        if (checkList == null)
            throw new NotFoundException(MessageCode.CheckListIsNotExists, request.Id.ToString());

        return new GetCheckListByIdResult(mapper.Map<Lab.Application.Dtos.CheckLists.CheckListDto>(checkList));
    }
}
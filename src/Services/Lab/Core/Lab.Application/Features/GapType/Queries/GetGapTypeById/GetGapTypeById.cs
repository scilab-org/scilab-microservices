using AutoMapper;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.GapType.Queries.GetGapTypeById;

public record GetGapTypeByIdQuery(Guid Id) : ICommand<GetGapTypeByIdResult>;

public class GetGapTypeByIdQueryValidator : AbstractValidator<GetGapTypeByIdQuery>
{
    public GetGapTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetGapTypeByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetGapTypeByIdQuery, GetGapTypeByIdResult>
{
    public async Task<GetGapTypeByIdResult> Handle(GetGapTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<GapTypeEntity>(request.Id, cancellationToken);
        if (entity == null)
            throw new NotFoundException(MessageCode.NotFound, request.Id.ToString());

        return new GetGapTypeByIdResult(mapper.Map<Lab.Application.Dtos.GapTypes.GapTypeDto>(entity));
    }
}

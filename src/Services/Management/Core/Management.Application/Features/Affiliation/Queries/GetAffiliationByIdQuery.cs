using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Affiliation.Queries;

public sealed record GetAffiliationByIdQuery(Guid Id) : IRequest<AffiliationDto>;

public sealed class GetAffiliationByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetAffiliationByIdQuery, AffiliationDto>
{
    public async Task<AffiliationDto> Handle(GetAffiliationByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<AffiliationEntity>(query.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.AffiliationIsNotExists, query.Id.ToString());

        return mapper.Map<AffiliationDto>(entity);
    }
}

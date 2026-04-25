using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.UserAffiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Queries;

public sealed record GetUserAffiliationByIdQuery(Guid Id) : IRequest<UserAffiliationDto>;

public sealed class GetUserAffiliationByIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetUserAffiliationByIdQuery, UserAffiliationDto>
{
    public async Task<UserAffiliationDto> Handle(GetUserAffiliationByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await session.LoadAsync<UserAffiliationEntity>(query.Id, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.UserAffiliationIsNotExists, query.Id.ToString());

        var dto = mapper.Map<UserAffiliationDto>(entity);
        var affiliation = await session.LoadAsync<AffiliationEntity>(entity.AffiliationId, cancellationToken);
        dto.Affiliation = affiliation is null ? null : mapper.Map<AffiliationDto>(affiliation);

        return dto;
    }
}

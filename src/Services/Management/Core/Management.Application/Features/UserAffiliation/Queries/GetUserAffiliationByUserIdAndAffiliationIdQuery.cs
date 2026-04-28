using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.UserAffiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Queries;

public sealed record GetUserAffiliationByUserIdAndAffiliationIdQuery(Guid UserId, Guid AffiliationId) : IRequest<UserAffiliationDto>;

public sealed class GetUserAffiliationByUserIdAndAffiliationIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetUserAffiliationByUserIdAndAffiliationIdQuery, UserAffiliationDto>
{
    public async Task<UserAffiliationDto> Handle(GetUserAffiliationByUserIdAndAffiliationIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await session.Query<UserAffiliationEntity>()
            .FirstOrDefaultAsync(x => x.UserId == query.UserId && x.AffiliationId == query.AffiliationId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.UserAffiliationIsNotExists, $"{query.UserId}:{query.AffiliationId}");

        var dto = mapper.Map<UserAffiliationDto>(entity);
        var affiliation = await session.LoadAsync<AffiliationEntity>(entity.AffiliationId, cancellationToken);
        dto.Affiliation = affiliation is null ? null : mapper.Map<AffiliationDto>(affiliation);

        return dto;
    }
}

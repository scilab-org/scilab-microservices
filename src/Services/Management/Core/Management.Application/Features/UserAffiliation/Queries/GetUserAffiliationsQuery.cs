using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Dtos.UserAffiliations;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.UserAffiliation.Queries;

public sealed record GetUserAffiliationsQuery() : IRequest<List<UserAffiliationDto>>;

public sealed class GetUserAffiliationsQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetUserAffiliationsQuery, List<UserAffiliationDto>>
{
    public async Task<List<UserAffiliationDto>> Handle(GetUserAffiliationsQuery query, CancellationToken cancellationToken)
    {
        var result = await session.Query<UserAffiliationEntity>()
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var items = new List<UserAffiliationDto>(result.Count);
        foreach (var item in result)
        {
            var dto = mapper.Map<UserAffiliationDto>(item);
            var affiliation = await session.LoadAsync<AffiliationEntity>(item.AffiliationId, cancellationToken);
            dto.Affiliation = affiliation is null ? null : mapper.Map<AffiliationDto>(affiliation);
            items.Add(dto);
        }

        return items;
    }
}

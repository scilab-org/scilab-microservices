using AutoMapper;
using Management.Application.Dtos.Affiliations;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Linq;
using MediatR;

namespace Management.Application.Features.Affiliation.Queries;

public sealed record GetAffiliationsQuery(PaginationRequest Paging, string? Name) : IRequest<GetAffiliationsResult>;

public sealed class GetAffiliationsQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetAffiliationsQuery, GetAffiliationsResult>
{
    public async Task<GetAffiliationsResult> Handle(GetAffiliationsQuery query, CancellationToken cancellationToken)
    {
        var affiliationQuery = session.Query<AffiliationEntity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();
            affiliationQuery = affiliationQuery.Where(x => x.Name.ToLower().Contains(name));
        }

        var totalCount = await affiliationQuery.CountAsync(cancellationToken);

        var result = await affiliationQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<AffiliationDto>>(result);

        return new GetAffiliationsResult(items, totalCount, query.Paging);
    }
}

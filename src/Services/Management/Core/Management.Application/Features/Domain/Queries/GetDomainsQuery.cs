using AutoMapper;
using Management.Application.Dtos.Domains;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Domain.Queries;

public sealed record GetDomainsQuery(PaginationRequest Paging, string? Name) : IRequest<GetDomainsResult>;

public sealed class GetDomainsQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetDomainsQuery, GetDomainsResult>
{
    public async Task<GetDomainsResult> Handle(GetDomainsQuery query, CancellationToken cancellationToken)
    {
        var domainQuery = session.Query<DomainEntity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();
            domainQuery = domainQuery.Where(x => x.Name.ToLower().Contains(query.Name));
        }

        var totalCount = await domainQuery.CountAsync(cancellationToken);

        var result = await domainQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<DomainDto>>(result);

        return new GetDomainsResult(items, totalCount, query.Paging);
    }
}

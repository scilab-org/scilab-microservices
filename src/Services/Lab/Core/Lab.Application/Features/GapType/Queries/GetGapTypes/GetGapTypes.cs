using AutoMapper;
using Lab.Application.Dtos.GapTypes;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.GapType.Queries.GetGapTypes;

public sealed record GetGapTypesQuery(PaginationRequest Paging, string? Name) : IRequest<GetGapTypesResult>;

public sealed class GetGapTypesQueryHandler(IDocumentSession session, IMapper mapper)
    : IRequestHandler<GetGapTypesQuery, GetGapTypesResult>
{
    public async Task<GetGapTypesResult> Handle(GetGapTypesQuery query, CancellationToken cancellationToken)
    {
        var gapTypeQuery = session.Query<GapTypeEntity>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();
            gapTypeQuery = gapTypeQuery.Where(x => x.Name.ToLower().Contains(name));
        }

        var totalCount = await gapTypeQuery.CountAsync(cancellationToken);

        var result = await gapTypeQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<GapTypeDto>>(result);

        return new GetGapTypesResult(items, totalCount, query.Paging);
    }
}

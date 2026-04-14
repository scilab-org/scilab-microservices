using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.Journal.Queries.GetJournals;

public record GetJournalsQuery(GetJournalsFilter Filter, PaginationRequest Paging) : IQuery<GetJournalsResult>;

public class GetJournalsQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetJournalsQuery, GetJournalsResult>
{
    public async Task<GetJournalsResult> Handle(GetJournalsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<ConferenceJournalEntity>().AsQueryable();

        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name.Contains(name));
        }

        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        {
            query = query.Where(x => x.IsDeleted());
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var journals = results.ToList();
        var items = mapper.Map<List<JournalDto>>(journals);

        return new GetJournalsResult(items, totalCount, paging);
    }
}
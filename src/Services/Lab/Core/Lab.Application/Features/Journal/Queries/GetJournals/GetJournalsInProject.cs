using AutoMapper;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Marten;

namespace Lab.Application.Features.Journal.Queries.GetJournals;

public record GetJournalsInProjectQuery(GetJournalsFilter Filter, PaginationRequest Paging, Guid ProjectId) : IQuery<GetJournalsResult>;

public class GetJournalsInProjectQueryHandler(IDocumentSession session, IMapper mapper) : IQueryHandler<GetJournalsInProjectQuery, GetJournalsResult>
{
    #region Implementations

    public async Task<GetJournalsResult> Handle(GetJournalsInProjectQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        // var query = session.Query<ConferenceJournalEntity>()
        //     .Where(x => x.ProjectIds.Contains(filter.ProjectId))
        //     .AsQueryable();
        //
        // if (!filter.Name.IsNullOrWhiteSpace())
        // {
        //     var name = filter.Name.Trim();
        //     query = query.Where(x => x.Name.Contains(name));
        // }
        //
        // if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        // {
        //     query = query.Where(x => x.IsDeleted());
        // }
        //
        // var totalCount = await query.CountAsync(cancellationToken);
        // var results = await query
        //     .OrderByDescending(x => x.CreatedOnUtc)
        //     .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        // var journals = results.ToList();
        // var items = mapper.Map<List<JournalDto>>(journals);

        // var response = new GetJournalsResult(items, totalCount, paging);
        var response = new GetJournalsResult(new List<JournalDto>(), 0, paging);
        return response;
    }

    #endregion
}
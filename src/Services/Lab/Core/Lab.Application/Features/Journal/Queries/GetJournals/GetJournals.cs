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

public class GetJournalsQueryHandler(
    IDocumentSession session,
    IMapper mapper)
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

        if (filter.TemplateId.HasValue && filter.TemplateId.Value != Guid.Empty)
        {
            query = query.Where(x => x.TemplateIds != null && x.TemplateIds.Contains(filter.TemplateId.Value));
        }

        if (filter.ProjectId.HasValue && filter.ProjectId.Value != Guid.Empty)
        {
            query = query.Where(x => x.ProjectIds != null && x.ProjectIds.Contains(filter.ProjectId.Value));
        }

        if (filter.PaperId.HasValue && filter.PaperId.Value != Guid.Empty)
        {
            query = query.Where(x => x.PaperIds != null && x.PaperIds.Contains(filter.PaperId.Value));
        }

        if (!filter.ISSN.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.ISSN != null && x.ISSN.Contains(filter.ISSN));
        }

        if (!filter.Ranking.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.Ranking != null && x.Ranking.Contains(filter.Ranking));
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(x => x.Type == filter.Type);
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

        if (items.Count > 0)
        {
            var allTemplateIds = journals
                .SelectMany(x => x.TemplateIds ?? [])
                .Distinct()
                .ToList();

            var templates = allTemplateIds.Count > 0
                ? await session.Query<TemplateEntity>()
                    .Where(x => allTemplateIds.Contains(x.Id))
                    .ToListAsync(cancellationToken)
                : [];

            var templateMap = templates.ToDictionary(x => x.Id, x => new JournalTemplateDto
            {
                Id = x.Id,
                Code = x.Code!
            });

            foreach (var (item, journal) in items.Zip(journals))
            {
                item.Templates = (journal.TemplateIds ?? [])
                    .Where(templateMap.ContainsKey)
                    .Select(id => templateMap[id])
                    .ToList();
            }
        }

        return new GetJournalsResult(items, totalCount, paging);
    }
}
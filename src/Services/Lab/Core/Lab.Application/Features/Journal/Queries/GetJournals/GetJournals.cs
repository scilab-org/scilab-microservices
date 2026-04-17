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

        if (!filter.TemplateCode.IsNullOrWhiteSpace())
        {
            var code = filter.TemplateCode.Trim().ToLower();
            var template = await session.Query<TemplateEntity>()
                .FirstOrDefaultAsync(x => x.Code!.ToLower().Contains(code), cancellationToken);
            var templateId = template?.Id ?? Guid.Empty;

            query = query.Where(x => x.TemplateId == templateId);
        }

        if (!filter.ProjectId.IsNullOrWhiteSpace())
        {
            if (Guid.TryParse(filter.ProjectId, out var projectId))
            {
                query = query.Where(x => x.ProjectIds != null && x.ProjectIds.Contains(projectId));
            }
        }

        if (!filter.PaperId.IsNullOrWhiteSpace())
        {
            if (Guid.TryParse(filter.PaperId, out var paperId))
            {
                query = query.Where(x => x.PaperIds != null && x.PaperIds.Contains(paperId));
            }
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
            foreach (var item in items)
            {
                var template = await session.LoadAsync<TemplateEntity>(item.TemplateId, cancellationToken);
                item.TemplateCode = template?.Code ?? "N/A";
            }
        }

        return new GetJournalsResult(items, totalCount, paging);
    }
}
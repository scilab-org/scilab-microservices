using AutoMapper;
using JasperFx.Core;
using Lab.Application.Dtos.Journals;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.Journal.Queries.GetJournals;

public record GetJournalsQuery(GetJournalsFilter Filter, PaginationRequest Paging) : IQuery<GetJournalsResult>;

public class GetJournalsQueryHandler(
    IDocumentSession session,
    IManagementApiService managementApiService,
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

        if (!filter.ProjectName.IsNullOrWhiteSpace() || !filter.ProjectCode.IsNullOrWhiteSpace())
        {
            var projects = await managementApiService.GetProjectsAsync(
                name: filter.ProjectName,
                code: filter.ProjectCode,
                pageNumber: 1,
                pageSize: 1000,
                cancellationToken);
            if (projects.Count > 0)
            {
                var projectIds = projects.Select(x => x.Id).ToList();
                query = query.Where(x => x.ProjectIds != null && x.ProjectIds.Any(projectIds.Contains));
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
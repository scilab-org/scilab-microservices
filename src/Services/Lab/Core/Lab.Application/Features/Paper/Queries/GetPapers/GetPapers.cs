using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.Paper.Queries.GetPapers;

public record GetPapersQuery(GetPapersFilter Filter, PaginationRequest Paging) : IQuery<GetPapersResult>;

public class GetPapersQueryHandler(IDocumentSession session, IMapper mapper) : IQueryHandler<GetPapersQuery, GetPapersResult>
{
    #region Implementations

    public async Task<GetPapersResult> Handle(GetPapersQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<PaperEntity>().AsQueryable();

        #region Query Filters

        if (!filter.Title.IsNullOrWhiteSpace())
        {
            var title = filter.Title.Trim();
            query = query.Where(x => x.Title.Contains(title));
        }

        if (!filter.Template.IsNullOrWhiteSpace())
        {
            var template = filter.Template.Trim();
            query = query.Where(x => x.Template != null && x.Template.Contains(template));
        }

        if (!filter.Context.IsNullOrWhiteSpace())
        {
            var context = filter.Context.Trim();
            query = query.Where(x => x.Context != null && x.Context.Contains(context));
        }

        if (!filter.Abstract.IsNullOrWhiteSpace())
        {
            var abstractText = filter.Abstract.Trim();
            query = query.Where(x => x.Abstract != null && x.Abstract.Contains(abstractText));
        }

        if (!filter.ResearchGap.IsNullOrWhiteSpace())
        {
            var researchGap = filter.ResearchGap.Trim();
            query = query.Where(x => x.ResearchGap != null && x.ResearchGap.Contains(researchGap));
        }

        if (!filter.MainContribution.IsNullOrWhiteSpace())
        {
            var mainContribution = filter.MainContribution.Trim();
            query = query.Where(x => x.MainContribution != null && x.MainContribution.Contains(mainContribution));
        }

        if (!filter.ResearchAim.IsNullOrWhiteSpace())
        {
            var researchAim = filter.ResearchAim.Trim();
            query = query.Where(x => x.ResearchAim != null && x.ResearchAim.Contains(researchAim));
        }

        if (!filter.GapType.IsNullOrWhiteSpace())
        {
            var gapType = filter.GapType.Trim();
            query = query.Where(x => x.GapType != null && x.GapType.Contains(gapType));
        }

        if (filter.ConferenceJournalId.HasValue)
        {
            query = query.Where(x => x.ConferenceJournalId == filter.ConferenceJournalId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        {
            query = query.Where(x => x.IsDeleted());
        }

        #endregion

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var papers = result.ToList();
        var items = mapper.Map<List<PaperDto>>(papers);

        return new GetPapersResult(items, totalCount, paging);
    }

    #endregion
}

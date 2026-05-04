using AutoMapper;
using Lab.Application.Dtos.CheckLists;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.CheckList.Queries.GetCheckLists;

public record GetCheckListsQuery(GetCheckListsFilter Filter, PaginationRequest Paging) : IQuery<GetCheckListsResult>;

public class GetCheckListsQueryHandler(
    IDocumentSession session,
    IMapper mapper)
    : IQueryHandler<GetCheckListsQuery, GetCheckListsResult>
{
    public async Task<GetCheckListsResult> Handle(GetCheckListsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<CheckListEntity>().AsQueryable();

        if (!filter.Section.IsNullOrWhiteSpace())
        {
            var section = filter.Section.ToLower().Trim();
            query = query.Where(x => x.Section.ToLower().Contains(section));
        }

        if (!filter.RuleName.IsNullOrWhiteSpace())
        {
            var ruleName = filter.RuleName.ToLower().Trim();
            query = query.Where(x => x.RuleName.ToLower().Contains(ruleName));
        }

        if (!filter.Item.IsNullOrWhiteSpace())
        {
            var item = filter.Item.Trim();
            query = query.Where(x => x.Item.Contains(item));
        }

        if (filter.Weight.HasValue)
        {
            query = query.Where(x => x.Weight == filter.Weight.Value);
        }

        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        {
            query = query.Where(x => x.IsDeleted());
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var items = mapper.Map<List<CheckListDto>>(results.ToList());

        return new GetCheckListsResult(items, totalCount, paging);
    }
}
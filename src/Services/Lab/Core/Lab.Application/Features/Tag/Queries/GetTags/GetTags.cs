using AutoMapper;
using Lab.Application.Dtos.Tags;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.Tag.Queries.GetTags;

public record GetTagsQuery(GetTagsFilter Filter, PaginationRequest Paging) : IQuery<GetTagsResult>;

public class GetTagsQueryHandler(IDocumentSession session, IMapper mapper) : IQueryHandler<GetTagsQuery, GetTagsResult>
{
    #region Implementations

    public async Task<GetTagsResult> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<TagEntity>().AsQueryable();

        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name.Contains(name));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var results = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var tags = results.ToList();
        var items = mapper.Map<List<TagDto>>(tags);

        var response = new GetTagsResult(items, totalCount, paging);

        return response;
    }

    #endregion
}
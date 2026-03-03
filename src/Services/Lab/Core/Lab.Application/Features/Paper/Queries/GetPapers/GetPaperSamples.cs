using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.Paper.Queries.GetPapers;

public record GetPaperSamplesQuery(GetPaperSamplesFilter Filter, PaginationRequest Paging) : IQuery<GetPapersResult>;


public class GetPaperSamplesQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetPaperSamplesQuery, GetPapersResult>
{
    #region Implementations

    public async Task<GetPapersResult> Handle(GetPaperSamplesQuery request, CancellationToken cancellationToken)
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

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }
        
        // Exclude Draft and Processing papers by default
        query = query.Where(x => x.Status != PaperStatus.Draft && x.Status != PaperStatus.Processing);

        #endregion

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var papers = result.ToList();
        var items = mapper.Map<List<PaperDto>>(papers);

        var reponse = new GetPapersResult(items, totalCount, paging);

        return reponse;
    }

    #endregion

    #region Methods

    private List<string> NomalizeTagNames(string[]? tagNames)
    {
        if (tagNames == null) return new List<string>();

        return tagNames.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    #endregion
}
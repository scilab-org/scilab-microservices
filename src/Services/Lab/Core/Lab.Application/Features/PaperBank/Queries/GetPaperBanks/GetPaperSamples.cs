using AutoMapper;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.PaperBank.Queries.GetPaperBanks;

public record GetPaperSamplesQuery(GetPaperSamplesFilter Filter, PaginationRequest Paging) : IQuery<GetPaperBanksResult>;


public class GetPaperSamplesQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetPaperSamplesQuery, GetPaperBanksResult>
{
    #region Implementations

    public async Task<GetPaperBanksResult> Handle(GetPaperSamplesQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<PaperBankEntity>().AsQueryable();

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

        var reponse = new GetPaperBanksResult(items, totalCount, paging);

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
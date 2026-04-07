using AutoMapper;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.PaperBank.Queries.GetPaperBanks;

public record GetPaperBanksQuery(GetPaperBanksFilter Filter, PaginationRequest Paging) : IQuery<GetPaperBanksResult>;

public class GetPaperBanksQueryHandler(IDocumentSession session, IMapper mapper) : IQueryHandler<GetPaperBanksQuery, GetPaperBanksResult>
{
    #region Implementations

    public async Task<GetPaperBanksResult> Handle(GetPaperBanksQuery request, CancellationToken cancellationToken)
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

        if (filter.Author?.Any() == true)
        {
            var authorKeywords = NormalizeAuthorKeywords(filter.Author);

            foreach (var keyword in authorKeywords)
            {
                var local = keyword;
                query = query.Where(x => x.Authors != null && x.Authors.ToLower().Contains(local));
            }
        }

        if (!filter.Publisher.IsNullOrWhiteSpace())
        {
            var publisher = filter.Publisher.Trim();
            query = query.Where(x => x.Publisher != null && x.Publisher.Contains(publisher));
        }

        if (!filter.Abstract.IsNullOrWhiteSpace())
        {
            var abstractText = filter.Abstract.Trim();
            query = query.Where(x => x.Abstract != null && x.Abstract.Contains(abstractText));
        }

        if (!filter.Doi.IsNullOrWhiteSpace())
        {
            var doi = filter.Doi.Trim();
            query = query.Where(x => x.Doi != null && x.Doi.Contains(doi));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.FromPublicationDate.HasValue)
        {
            query = query.Where(x => x.PublicationDate.HasValue && x.PublicationDate.Value >= filter.FromPublicationDate.Value);
        }

        if (filter.ToPublicationDate.HasValue)
        {
            query = query.Where(x => x.PublicationDate.HasValue && x.PublicationDate.Value <= filter.ToPublicationDate.Value);
        }

        if (!filter.PaperType.IsNullOrWhiteSpace())
        {
            var paperType = filter.PaperType.Trim();
            query = query.Where(x => x.PaperType != null && x.PaperType.Contains(paperType));
        }

        if (!filter.JournalName.IsNullOrWhiteSpace())
        {
            var journalName = filter.JournalName.Trim();
            query = query.Where(x => x.JournalName != null && x.JournalName.Contains(journalName));
        }

        if (!filter.ConferenceName.IsNullOrWhiteSpace())
        {
            var conferenceName = filter.ConferenceName.Trim();
            query = query.Where(x => x.ConferenceName != null && x.ConferenceName.Contains(conferenceName));
        }


        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        {
            query = query.Where(x => x.IsDeleted());
        }

        if (filter.Tag?.Any() == true)
        {
            var tagNames = NormalizeTagNames(filter.Tag);

            foreach (var searchTag in tagNames)
            {
                var local = searchTag;

                query = query.Where(p =>
                    p.TagNames.Count != 0 &&
                    p.TagNames.Any(t => t.Contains(local))
                );
            }
        }

        // Exclude Draft and Processing papers by default
        query = query.Where(x => x.Status != PaperStatus.Draft && x.Status != PaperStatus.Processing);

        #endregion

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var papers = result.ToList();
        var items = mapper.Map<List<PaperBankDto>>(papers);

        var response = new GetPaperBanksResult(items, totalCount, paging);

        return response;
    }

    #endregion

    #region Methods

    private List<string> NormalizeTagNames(string[]? tagNames)
    {
        if (tagNames == null) return new List<string>();

        return tagNames.Select(x => x.Trim().ToLowerInvariant()).ToList();
    }

    private List<string> NormalizeAuthorKeywords(string[]? authors)
    {
        if (authors == null) return new List<string>();

        return authors
            .SelectMany(x => x
                .Trim()
                .ToLowerInvariant()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToList();
    }

    #endregion
}
using AutoMapper;
using Lab.Application.Dtos.PaperBanks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;

namespace Lab.Application.Features.PaperBank.Queries.GetPaperBanks;

public record GetPaperBanksQuery(GetPaperBanksFilter Filter, PaginationRequest Paging) : IQuery<GetPaperBanksResult>;

public class GetPaperBanksQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetPaperBanksQuery, GetPaperBanksResult>
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
            var title = filter.Title.Trim().ToLower();
            query = query.Where(x => x.Title != null! && x.Title.ToLower().Contains(title));
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

        if (filter.FromPublicationDate.HasValue)
        {
            query = query.Where(x =>
                x.PublicationDate.HasValue && x.PublicationDate.Value >= filter.FromPublicationDate.Value);
        }

        if (filter.ToPublicationDate.HasValue)
        {
            query = query.Where(x =>
                x.PublicationDate.HasValue && x.PublicationDate.Value <= filter.ToPublicationDate.Value);
        }

        if (!filter.PaperType.IsNullOrWhiteSpace())
        {
            var paperType = filter.PaperType.Trim();
            query = query.Where(x => x.PaperType != null && x.PaperType.Contains(paperType));
        }

        if (filter.JournalId.HasValue)
        {
            query = query.Where(x => x.ConferenceJournalId != null && x.ConferenceJournalId == filter.JournalId);
        }

        if (!filter.Ranking.IsNullOrWhiteSpace())
        {
            var ranking = filter.Ranking.Trim();
            query = query.Where(x => x.Ranking != null && x.Ranking.Contains(ranking));
        }


        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
        {
            query = query.Where(x => x.IsDeleted());
        }

        #region Filter for endpoint GetAvailablePapers

        if (filter.ExistingPaperIds?.Any() == true)
        {
            var ids = filter.ExistingPaperIds.ToList();
            query = query.Where(x => !ids.Contains(x.Id));
        }

        if (filter.Keyword?.Any() == true)
        {
            var keywords = NormalizeKeywords(filter.Keyword);

            if (keywords.Count > 0)
            {
                foreach (var searchKeyword in keywords)
                {
                    var local = searchKeyword;

                    query = query.Where(p =>
                        p.Keywords.Count != 0 &&
                        p.Keywords.Any(t => t.Contains(local))
                    );
                }
            }
        }

        #endregion

        #endregion

        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var papers = result.ToList();
        var items = mapper.Map<List<PaperBankDto>>(papers);

        var journalIds = papers.Select(p => p.ConferenceJournalId).ToList();
        var journals = await session.Query<ConferenceJournalEntity>()
            .Where(x => journalIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        items.ForEach(item =>
        {
            var journal = journals.FirstOrDefault(x => x.Id == item.ConferenceJournalId);
            if (journal != null)
            {
                item.ConferenceJournalName = journal.Name;
            }
        });

        var response = new GetPaperBanksResult(items, totalCount, paging);

        return response;
    }

    #endregion

    #region Methods

    private List<string> NormalizeKeywords(string[]? keywords)
    {
        if (keywords == null) return new List<string>();

        return keywords
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .ToList();
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
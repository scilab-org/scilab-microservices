using AutoMapper;
using Lab.Application.Dtos.Papers;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;
using MediatR;

namespace Lab.Application.Features.Paper.Queries.GetPapers;

public record GetPapersQuery(GetPapersFilter Filter, PaginationRequest Paging) : IQuery<GetPapersResult>;

public class GetPapersQueryValidator : AbstractValidator<GetPapersQuery>
{
    public GetPapersQueryValidator()
    {
    }
}

public class GetPapersQueryHandler(IDocumentSession session, IMapper mapper) : IQueryHandler<GetPapersQuery, GetPapersResult>
{
    public async Task<GetPapersResult> Handle(GetPapersQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var paperQuery = session.Query<PaperEntity>().AsQueryable();

        if (!filter.Title.IsNullOrWhiteSpace())
        {
            var title = filter.Title.Trim();
            paperQuery = paperQuery.Where(x => x.Title.Contains(title));
        }

        if (!filter.Abstract.IsNullOrWhiteSpace())
        {
            var abstractText = filter.Abstract.Trim();
            paperQuery = paperQuery.Where(x => x.Abstract != null && x.Abstract.Contains(abstractText));
        }

        if (!filter.Doi.IsNullOrWhiteSpace())
        {
            var doi = filter.Doi.Trim();
            paperQuery = paperQuery.Where(x => x.Doi != null && x.Doi.Contains(doi));
        }

        if (filter.Status.HasValue)
        {
            paperQuery = paperQuery.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.FromPublicationDate.HasValue)
        {
            paperQuery = paperQuery.Where(x => x.PublicationDate.HasValue && x.PublicationDate.Value >= filter.FromPublicationDate.Value);
        }

        if (filter.ToPublicationDate.HasValue)
        {
            paperQuery = paperQuery.Where(x => x.PublicationDate.HasValue && x.PublicationDate.Value <= filter.ToPublicationDate.Value);
        }

        if (!filter.PaperType.IsNullOrWhiteSpace())
        {
            var paperType = filter.PaperType.Trim();
            paperQuery = paperQuery.Where(x => x.PaperType != null && x.PaperType.Contains(paperType));
        }

        if (!filter.JournalName.IsNullOrWhiteSpace())
        {
            var journalName = filter.JournalName.Trim();
            paperQuery = paperQuery.Where(x => x.JournalName != null && x.JournalName.Contains(journalName));
        }

        if (!filter.ConferenceName.IsNullOrWhiteSpace())
        {
            var conferenceName = filter.ConferenceName.Trim();
            paperQuery = paperQuery.Where(x => x.ConferenceName != null && x.ConferenceName.Contains(conferenceName));
        }

        var totalCount = await paperQuery.CountAsync(cancellationToken);
        var result = await paperQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var papers = result.ToList();
        var items = mapper.Map<List<PaperDto>>(papers);

        var reponse = new GetPapersResult(items, totalCount, paging);

        return reponse;
    }
}
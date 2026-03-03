using AutoMapper;
using Lab.Application.Dtos.Template;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.Template.Queries.GetTemplates;

public record GetTemplatesQuery(GetTemplatesFilter Filter, PaginationRequest Paging) : IQuery<GetTemplatesResult>;

public class GetTemplatesQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetTemplatesQuery, GetTemplatesResult>
{
    #region Implementations

    public async Task<GetTemplatesResult> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<TemplateEntity>().AsQueryable();
       
        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name.Contains(name));
        }

        if (!filter.Code.IsNullOrWhiteSpace())
        {
            var code = filter.Code.Trim();
            query = query.Where(x => x.Code != null && x.Code.Contains(code));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var results = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var tempalates = results.ToList();

        var items = mapper.Map<List<TemplateDto>>(tempalates);

        return new GetTemplatesResult(items, totalCount, paging);
    }

    #endregion
}



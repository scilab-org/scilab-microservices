using AutoMapper;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Linq.SoftDeletes;
using Marten.Pagination;
using MediatR;

namespace Management.Application.Features.Project.Queries;

public sealed record GetProjectsQuery(
    GetProjectsFilter Filter,
    PaginationRequest Paging) : IQuery<GetProjectsResult>;

public sealed class GetProjectsQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetProjectsQuery, GetProjectsResult>
{
    #region Implementations

    public async Task<GetProjectsResult> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var paging = request.Paging;
        var query = session.Query<ProjectEntity>().AsQueryable().Where(x => x.ParentProjectId == null);

        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name != null && x.Name.Contains(name));
        }
        if(!filter.Code.IsNullOrWhiteSpace())
        {
            var code = filter.Code.Trim();
            query = query.Where(x => x.Code != null && x.Code.Contains(code));
        }
        if (filter.Status.HasValue)        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }
        if (filter.IsDeleted.HasValue && filter.IsDeleted.Value)
            query = query.Where(x => x.IsDeleted());
        
        
        var totalCount = await query.CountAsync(cancellationToken);
        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var projects = result.ToList();
        var items = mapper.Map<List<ProjectDto>>(projects);

        var reponse = new GetProjectsResult(items, totalCount, paging);

        return reponse;
    }
    
    
    #endregion
}
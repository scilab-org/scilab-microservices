using AutoMapper;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
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

    public async Task<GetProjectsResult> Handle(GetProjectsQuery query, CancellationToken cancellationToken)
    {
        var filter = query.Filter;
        var paging = query.Paging;
        var projectQuery = session.Query<ProjectEntity>().AsQueryable();

        if (!filter.SearchText.IsNullOrWhiteSpace())
        {
            var search = filter.SearchText.Trim();
            projectQuery = projectQuery.Where(x => x.Name != null && x.Name.Contains(search));
        }

        var totalCount = await projectQuery.CountAsync(cancellationToken);
        var result = await projectQuery
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var projects = result.ToList();
        var items = mapper.Map<List<ProjectDto>>(projects);

        var reponse = new GetProjectsResult(items, totalCount, paging);

        return reponse;
    }
    
    
    #endregion
}
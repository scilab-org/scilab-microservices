using AutoMapper;
using Management.Application.Dtos.Domains;
using Management.Application.Dtos.Projects;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Management.Application.Features.Project.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetMyProjectsQuery(
    Guid UserId,
    PaginationRequest Paging,
    GetMyProjectsFilter Filter) : IQuery<GetProjectsResult>;

public sealed class GetMyProjectsValidator : AbstractValidator<GetMyProjectsQuery>
{
    public GetMyProjectsValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }
}

[ExcludeFromCodeCoverage]
public sealed class GetMyProjectsQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetMyProjectsQuery, GetProjectsResult>
{
    #region Implementations

    public async Task<GetProjectsResult> Handle(GetMyProjectsQuery request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var paging = request.Paging;
        var filter = request.Filter;

        var memberProjectIds = await session.Query<MemberEntity>()
            .Where(x => x.UserId == userId)
            .Select(x => x.ProjectId)
            .ToListAsync(cancellationToken);

        var query = session.Query<ProjectEntity>()
            .Where(x => memberProjectIds.Contains(x.Id) && x.ParentProjectId == null)
            .AsQueryable();

        if (!filter.Name.IsNullOrWhiteSpace())
        {
            var name = filter.Name.Trim();
            query = query.Where(x => x.Name != null && x.Name.Contains(name));
        }

        if (!filter.Code.IsNullOrWhiteSpace())
        {
            var code = filter.Code.Trim();
            query = query.Where(x => x.Code != null && x.Code.Contains(code));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToPagedListAsync(paging.PageNumber, paging.PageSize, cancellationToken);

        var projects = result.ToList();
        var items = mapper.Map<List<ProjectDto>>(projects);
        await PopulateDomainsAsync(projects, items, cancellationToken);

        return new GetProjectsResult(items, totalCount, paging);
    }

    private async Task PopulateDomainsAsync(
        List<ProjectEntity> projects,
        List<ProjectDto> projectDtos,
        CancellationToken cancellationToken)
    {
        var domainIds = projects
            .SelectMany(x => x.DomainIds ?? [])
            .Distinct()
            .ToList();

        if (domainIds.Count == 0) return;

        var domains = await session.Query<DomainEntity>()
            .Where(x => domainIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var domainMap = mapper.Map<List<DomainDto>>(domains)
            .ToDictionary(x => x.Id, x => x);

        foreach (var projectDto in projectDtos)
        {
            var project = projects.FirstOrDefault(x => x.Id == projectDto.Id);
            if (project == null) continue;

            projectDto.Domains = project.DomainIds
                .Where(domainMap.ContainsKey)
                .Select(id => domainMap[id])
                .ToList();
        }
    }

    #endregion
}

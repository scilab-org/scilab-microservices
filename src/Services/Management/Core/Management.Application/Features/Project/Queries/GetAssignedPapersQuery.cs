using AutoMapper;
using Management.Application.Dtos.Papers;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Management.Application.Features.Project.Queries;

public sealed record GetAssignedPapersQuery(
    Guid UserId,
    PaginationRequest Paging,
    GetAssignedPapersFilter Filter) : IQuery<GetAssignedPapersResult>;

public sealed class GetAssignedPapersValidator : AbstractValidator<GetAssignedPapersQuery>
{
    public GetAssignedPapersValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdIsRequired);
    }
}

public sealed class GetAssignedPapersQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService)
    : IQueryHandler<GetAssignedPapersQuery, GetAssignedPapersResult>
{
    public async Task<GetAssignedPapersResult> Handle(GetAssignedPapersQuery request, CancellationToken cancellationToken)
    {
        var title = request.Filter.Title;

        var normalizedProjectName = request.Filter.ProjectName?.Trim();
        var normalizedProjectCode = request.Filter.ProjectCode?.Trim();

        var projectQuery = session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == null);

        if (!string.IsNullOrWhiteSpace(normalizedProjectName))
            projectQuery = projectQuery.Where(x => x.Name != null && x.Name.Contains(normalizedProjectName, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(normalizedProjectCode))
            projectQuery = projectQuery.Where(x => x.Code != null && x.Code.Contains(normalizedProjectCode, StringComparison.OrdinalIgnoreCase));

        var matchedParentProjectIds = await projectQuery
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (matchedParentProjectIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var projectIds = await session.Query<MemberEntity>()
            .Where(x => x.UserId == request.UserId)
            .Select(x => x.ProjectId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (projectIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        projectIds = projectIds
            .Where(matchedParentProjectIds.Contains)
            .ToList();

        if (projectIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var subProjectIds = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId != null && projectIds.Contains(x.ParentProjectId.Value))
            .Select(x => x.Id)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (subProjectIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var memberSubProjectIds = await session.Query<MemberEntity>()
            .Where(x => x.UserId == request.UserId && subProjectIds.Contains(x.ProjectId))
            .Select(x => x.ProjectId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (memberSubProjectIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => memberSubProjectIds.Contains(x.Id))
            .Select(x => new { x.Id, x.ParentProjectId, x.PaperIds })
            .ToListAsync(cancellationToken);

        var paperToProjectMap = new Dictionary<Guid, Guid>();
        foreach (var subProject in subProjects)
        {
            if (subProject.ParentProjectId.HasValue)
            {
                foreach (var paperId in subProject.PaperIds)
                {
                    paperToProjectMap[paperId] = subProject.ParentProjectId.Value;
                }
            }
        }

        var paperIds = paperToProjectMap.Keys.Distinct().ToList();

        if (paperIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var distinctParentProjectIds = paperToProjectMap.Values.Distinct().ToList();
        var parentProjects = await session.Query<ProjectEntity>()
            .Where(x => distinctParentProjectIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Code })
            .ToListAsync(cancellationToken);

        var projectIdToCodeMap = parentProjects.ToDictionary(x => x.Id, x => x.Code);

        var (items, totalCount) = await labApiService.GetPapersByIdsPagedAsync(
            paperIds,
            title: title,
            pageNumber: request.Paging.PageNumber,
            pageSize: request.Paging.PageSize,
            cancellationToken: cancellationToken);

        var assignedItems = items.Select(item => 
        {
            var projectId = paperToProjectMap.TryGetValue(item.Id, out var pid) ? pid : Guid.Empty;
            return new AssignedPaperDto
            {
                ProjectId = projectId,
                ProjectCode = projectId != Guid.Empty && projectIdToCodeMap.TryGetValue(projectId, out var code) ? code : null,
                Id = item.Id,
                SubProjectId = item.SubProjectId,
                Title = item.Title,
                Authors = item.Authors,
                Abstract = item.Abstract,
                FilePath = item.FilePath,
                Status = item.Status,
                CreatedBy = item.CreatedBy,
                Template = item.Template
            };
        }).ToList();

        return new GetAssignedPapersResult(assignedItems, totalCount, request.Paging);
    }
}

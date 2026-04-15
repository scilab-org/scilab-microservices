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

        var paperIds = await session.Query<ProjectEntity>()
            .Where(x => memberSubProjectIds.Contains(x.Id))
            .SelectMany(x => x.PaperIds)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (paperIds.Count == 0)
            return new GetAssignedPapersResult([], 0, request.Paging);

        var (items, totalCount) = await labApiService.GetPapersByIdsPagedAsync(
            paperIds,
            title: title,
            pageNumber: request.Paging.PageNumber,
            pageSize: request.Paging.PageSize,
            cancellationToken: cancellationToken);

        return new GetAssignedPapersResult(items, totalCount, request.Paging);
    }
}

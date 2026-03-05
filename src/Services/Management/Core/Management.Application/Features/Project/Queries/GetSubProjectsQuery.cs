using Management.Application.Dtos.Papers;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetSubProjectsQuery(
    Guid ProjectId,
    PaginationRequest Paging,
    string? Title = null) : IQuery<GetSubProjectsPapersResult>;

public class GetSubProjectsValidator : AbstractValidator<GetSubProjectsQuery>
{
    public GetSubProjectsValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.ProjectIdIsRequired);
    }
}

public class GetSubProjectsQueryHandler(
    IDocumentSession session,
    ILabApiService labApiService)
    : IQueryHandler<GetSubProjectsQuery, GetSubProjectsPapersResult>
{
    #region Implementations

    public async Task<GetSubProjectsPapersResult> Handle(
        GetSubProjectsQuery query,
        CancellationToken cancellationToken)
    {
        // Verify parent project exists
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Get all sub-projects where ParentProjectId == projectId
        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == query.ProjectId)
            .ToListAsync(cancellationToken);

        if (!subProjects.Any())
            return new GetSubProjectsPapersResult(
                new List<PaperInfoDto>(), 0, query.Paging);

        // Create a dictionary to map paperId -> subProjectId
        var paperToSubProjectMap = new Dictionary<Guid, Guid>();
        foreach (var subProject in subProjects)
        {
            foreach (var paperId in subProject.PaperIds)
            {
                if (!paperToSubProjectMap.ContainsKey(paperId))
                    paperToSubProjectMap[paperId] = subProject.Id;
            }
        }

        // Collect all distinct paper IDs across all sub-projects
        var allPaperIds = paperToSubProjectMap.Keys.ToList();

        if (!allPaperIds.Any())
            return new GetSubProjectsPapersResult(
                new List<PaperInfoDto>(), 0, query.Paging);

        // Fetch papers from Lab service with title filter and paging
        var (items, totalCount) = await labApiService.GetPapersByIdsPagedAsync(
            paperIds: allPaperIds,
            title: query.Title,
            pageNumber: query.Paging.PageNumber,
            pageSize: query.Paging.PageSize,
            cancellationToken: cancellationToken);

        // Map SubProjectId to each paper
        foreach (var paper in items)
        {
            if (paperToSubProjectMap.TryGetValue(paper.Id, out var subProjectId))
                paper.SubProjectId = subProjectId;
        }

        return new GetSubProjectsPapersResult(items, totalCount, query.Paging);
    }

    #endregion
}

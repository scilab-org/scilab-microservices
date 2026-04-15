using AutoMapper;
using BuildingBlocks.Pagination;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.TaskDefinition.Queries.GetTasksByPaperId;

public sealed record GetTasksByPaperIdQuery(Guid PaperId, GetTaskByPaperIdFilter Filter, PaginationRequest Paging) : IQuery<GetTasksPagedResult>;

public sealed class GetTasksByPaperIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetTasksByPaperIdQuery, GetTasksPagedResult>
{
    public async Task<GetTasksPagedResult> Handle(GetTasksByPaperIdQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        var contributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == request.PaperId && x.TaskIds.Any())
            .ToListAsync(cancellationToken);

        if (contributors.Count == 0)
            return new GetTasksPagedResult([], 0, request.Paging);

        var taskIds = contributors
            .SelectMany(x => x.TaskIds)
            .Distinct()
            .ToList();

        var query = session.Query<TaskEntity>()
            .Where(x => taskIds.Contains(x.Id));

        if (!string.IsNullOrWhiteSpace(filter.AssignedToUserName))
            query = query.Where(x => x.AssignedToUserName == filter.AssignedToUserName);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(x => x.CreatedOnUtc >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(x => x.CreatedOnUtc <= filter.ToDate.Value);

        var pagedTasks = await query.ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize, cancellationToken);
        var tasks = pagedTasks.ToList();
        var contributorMap = contributors
            .SelectMany(x => x.TaskIds.Select(taskId => new { taskId, contributor = x }))
            .GroupBy(x => x.taskId)
            .ToDictionary(g => g.Key, g => g.First().contributor);

        var paper = await session.LoadAsync<PaperEntity>(request.PaperId, cancellationToken);
        var paperName = paper?.Title ?? string.Empty;

        var sectionIds = contributors
            .Where(x => x.SectionId.HasValue)
            .Select(x => x.SectionId!.Value)
            .Distinct()
            .ToList();
        var sectionTitleMap = sectionIds.Count > 0
            ? (await session.Query<SectionEntity>()
                .Where(x => sectionIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Title })
                .ToListAsync(cancellationToken))
              .ToDictionary(x => x.Id, x => x.Title)
            : new Dictionary<Guid, string?>();

        var taskDtos = mapper.Map<List<TaskDto>>(tasks);
        foreach (var taskDto in taskDtos)
        {
            taskDto.PaperId = request.PaperId;
            taskDto.PaperTitle = paperName;

            if (contributorMap.TryGetValue(taskDto.Id, out var contributor))
            {
                taskDto.PaperContributorId = contributor.Id;
                taskDto.SectionId = contributor.SectionId;
                taskDto.SectionTitle = contributor.SectionId.HasValue && sectionTitleMap.TryGetValue(contributor.SectionId.Value, out var sTitle) ? sTitle : null;
            }
        }

        return new GetTasksPagedResult(taskDtos, pagedTasks.TotalItemCount, request.Paging);
    }
}

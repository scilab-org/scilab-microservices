using System.Linq.Expressions;
using AutoMapper;
using BuildingBlocks.Pagination;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;

namespace Lab.Application.Features.TaskDefinition.Queries.GetMyTask;

public sealed record GetMyTaskQuery(string UserName, GetTaskFilter Filter, PaginationRequest Paging): IQuery<GetTasksPagedResult>;


public class GetMyTaskQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetMyTaskQuery, GetTasksPagedResult>
{
    #region Implementations

    public async Task<GetTasksPagedResult> Handle(GetMyTaskQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        var query = session.Query<TaskEntity>()
            .Where(x => x.AssignedToUserName == request.UserName);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        #region Date Filtering
        
        if (filter.FromDate.HasValue || filter.ToDate.HasValue)
        {
            var field = filter.DateField ?? DateTaskFilterField.CreatedOn;
    
            (Expression<Func<TaskEntity, bool>>? fromExpr, Expression<Func<TaskEntity, bool>>? toExpr) = field switch
            {
                DateTaskFilterField.StartDate => (
                    filter.FromDate.HasValue ? (Expression<Func<TaskEntity, bool>>)(x => x.StartDate >= filter.FromDate) : null,
                    filter.ToDate.HasValue   ? (Expression<Func<TaskEntity, bool>>)(x => x.StartDate <= filter.ToDate)   : null
                ),
                DateTaskFilterField.NextReviewDate => (
                    filter.FromDate.HasValue ? (Expression<Func<TaskEntity, bool>>)(x => x.NextReviewDate >= filter.FromDate) : null,
                    filter.ToDate.HasValue   ? (Expression<Func<TaskEntity, bool>>)(x => x.NextReviewDate <= filter.ToDate)   : null
                ),
                DateTaskFilterField.CompleteDate => (
                    filter.FromDate.HasValue ? (Expression<Func<TaskEntity, bool>>)(x => x.CompleteDate >= filter.FromDate) : null,
                    filter.ToDate.HasValue   ? (Expression<Func<TaskEntity, bool>>)(x => x.CompleteDate <= filter.ToDate)   : null
                ),
                _ => (
                    filter.FromDate.HasValue ? (Expression<Func<TaskEntity, bool>>)(x => x.CreatedOnUtc >= filter.FromDate) : null,
                    filter.ToDate.HasValue   ? (Expression<Func<TaskEntity, bool>>)(x => x.CreatedOnUtc <= filter.ToDate)   : null
                )
            };

            if (fromExpr != null) query = query.Where(fromExpr);
            if (toExpr != null) query = query.Where(toExpr);
        }

        #endregion
        
        var pagedTasks = await query.ToPagedListAsync(request.Paging.PageNumber, request.Paging.PageSize, cancellationToken);
        var tasks = pagedTasks.ToList();
        if (!tasks.Any())
            return new GetTasksPagedResult([], 0, request.Paging);

        var taskIds = tasks.Select(x => x.Id).ToList();

        var paperContributorQuery = session.Query<PaperContributorEntity>()
            .Where(x => x.TaskIds.Any(tid => taskIds.Contains(tid))); 
        
        if (filter.PaperId.HasValue)
            paperContributorQuery = paperContributorQuery.Where(x => x.PaperId == filter.PaperId.Value);

        var contributors = await paperContributorQuery
            .ToListAsync(cancellationToken);

        var contributorMap = contributors
            .SelectMany(x => x.TaskIds
                .Where(tid => taskIds.Contains(tid))
                .Select(taskId => new { taskId, contributor = x }))
            .GroupBy(x => x.taskId)
            .ToDictionary(g => g.Key, g => g.First().contributor);

        var paperIds = contributors.Select(x => x.PaperId).Distinct().ToList();
        var paperNameMap = await session.Query<PaperEntity>()
            .Where(x => paperIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Title })
            .ToListAsync(cancellationToken);

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
        
        var filteredTasks = filter.PaperId.HasValue
            ? tasks.Where(x => contributorMap.ContainsKey(x.Id)).ToList()
            : tasks;

        var taskDtos = mapper.Map<List<TaskDto>>(filteredTasks);

        foreach (var taskDto in taskDtos)
        {
            if (!contributorMap.TryGetValue(taskDto.Id, out var contributor))
                continue;

            taskDto.SectionId = contributor.SectionId;
            taskDto.PaperId = contributor.PaperId;
            taskDto.PaperContributorId = contributor.Id;
            taskDto.PaperTitle = paperNameMap.FirstOrDefault(x => x.Id == contributor.PaperId)?.Title ?? string.Empty;
            taskDto.SectionTitle = contributor.SectionId.HasValue && sectionTitleMap.TryGetValue(contributor.SectionId.Value, out var sTitle) ? sTitle : null;
        }
        
        return new GetTasksPagedResult(taskDtos, pagedTasks.TotalItemCount, request.Paging);
    }

    #endregion
    
}

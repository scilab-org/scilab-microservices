using AutoMapper;
using BuildingBlocks.Pagination;
using Lab.Application.Dtos.Tasks;
using Lab.Application.Models.Filters;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;
using Marten.Pagination;
using Lab.Application.Services;

namespace Lab.Application.Features.TaskDefinition.Queries.GetTasksByPaperId;

public sealed record GetTasksByPaperIdQuery(Guid PaperId, string UserId, GetTaskByPaperIdFilter Filter, PaginationRequest Paging) : IQuery<GetTasksPagedResult>;

public sealed class GetTasksByPaperIdQueryHandler(
    IDocumentSession session, 
    IMapper mapper,
    IManagementApiService apiService,
    IUserApiService userApiService)
    : IQueryHandler<GetTasksByPaperIdQuery, GetTasksPagedResult>
{
    public async Task<GetTasksPagedResult> Handle(GetTasksByPaperIdQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        var memberInfo = await apiService.GetMemberByPaperIdAsync(request.PaperId, Guid.Parse(request.UserId), cancellationToken);
        if (memberInfo == null)
            throw new NoPermissionException(MessageCode.AccessDenied);

        var allMembers = await apiService.GetSubProjectMembersByPaperIdAsync(request.PaperId, cancellationToken);
        var memberIds = allMembers.Select(m => m.MemberId).ToList();

        if (memberIds.Count == 0)
            return new GetTasksPagedResult([], 0, request.Paging);

        // Resolve usernames from the User service for all members
        // var memberUserIds = allMembers.Select(m => m.UserId).Distinct().ToList();
        // var userInfoMap = await userApiService.GetUsersByIdsAsync(memberUserIds, cancellationToken);
        // var memberUsernameMap = allMembers
        //     .Where(m => userInfoMap.ContainsKey(m.UserId))
        //     .ToDictionary(m => m.MemberId, m => userInfoMap[m.UserId].Username);

        var query = session.Query<TaskEntity>()
            .Where(x => memberIds.Contains(x.MemberId));

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

        var contributors = await session.Query<PaperContributorEntity>()
            .Where(x => x.PaperId == request.PaperId)
            .ToListAsync(cancellationToken);

        // Map taskId → the contributor that owns it (each contributor records its TaskIds)
        var taskContributorMap = contributors
            .SelectMany(c => c.TaskIds.Select(tid => (tid, c)))
            .ToDictionary(x => x.tid, x => x.c);

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

            if (taskContributorMap.TryGetValue(taskDto.Id, out var contributor))
            {
                taskDto.PaperContributorId = contributor.Id;
                taskDto.SectionId = contributor.SectionId;
                taskDto.SectionTitle = contributor.SectionId.HasValue && sectionTitleMap.TryGetValue(contributor.SectionId.Value, out var sTitle) ? sTitle : null;
            }
        }

        return new GetTasksPagedResult(taskDtos, pagedTasks.TotalItemCount, request.Paging);
    }
}

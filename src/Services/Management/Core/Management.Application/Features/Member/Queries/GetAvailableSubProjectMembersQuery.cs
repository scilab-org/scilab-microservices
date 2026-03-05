using Management.Application.Dtos.Members;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

public sealed record GetAvailableSubProjectMembersQuery(
    Guid SubProjectId,
    GetAvailableSubProjectMembersFilter Filter,
    PaginationRequest Paging) : IQuery<GetProjectMembersResult>;


public class GetAvailableSubProjectMembersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetAvailableSubProjectMembersQuery, GetProjectMembersResult>
{
    #region Implementations

    public async Task<GetProjectMembersResult> Handle(
        GetAvailableSubProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        // Verify sub project exists
        var subProject = await session.LoadAsync<ProjectEntity>(request.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);
        
        // Load all members of the PARENT project
        var parentProjectMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == subProject.ParentProjectId)
            .ToListAsync(cancellationToken);
        
        // Load all members of the SUB-project (to exclude them)
        var subProjectMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == subProject.Id)
            .ToListAsync(cancellationToken);
        
        // Get UserIds that are already in subproject
        var subProjectUserIds = subProjectMembers.Select(x => x.UserId).ToHashSet();
        
        // Filter parent members: exclude those already in subproject
        var availableMembers = parentProjectMembers
            .Where(m => !subProjectUserIds.Contains(m.UserId))
            .ToList();
        
        if (!availableMembers.Any())
            return new GetProjectMembersResult(new List<ProjectMemberDto>(), 0, request.Paging);
        
        // Fetch user details from User service
        var userInfos = await userApiService.GetUsersByIdsAsync(
            userIds: availableMembers.Select(x => x.UserId),
            cancellationToken: cancellationToken);
        
        // Join member records with user info
        var joined = availableMembers
            .Join(userInfos,
                m => m.UserId.ToString(),
                u => u.Id,
                (m, u) => new ProjectMemberDto
                {
                    MemberId  = m.Id,
                    UserId    = m.UserId,
                    Username  = u.Username,
                    Email     = u.Email,
                    FirstName = u.FirstName,
                    LastName  = u.LastName,
                    Enabled   = u.Enabled,
                    Role      = m.ProjectRole,
                    JoinedAt  = m.JoinedAt
                })
            .Where(dto =>
                // Filter by email if provided
                (string.IsNullOrWhiteSpace(request.Filter.SearchEmail) ||
                 (dto.Email != null && dto.Email.Contains(request.Filter.SearchEmail, StringComparison.OrdinalIgnoreCase)))
                &&
                // Filter by ProjectRole if provided
                (string.IsNullOrWhiteSpace(request.Filter.ProjectRole) ||
                 string.Equals(dto.Role, request.Filter.ProjectRole, StringComparison.OrdinalIgnoreCase))
            )
            .OrderBy(dto => dto.JoinedAt)
            .ToList();
        
        var totalCount = joined.Count;
        
        // Apply pagination
        var paged = joined
            .Skip((request.Paging.PageNumber - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .ToList();
       
        return new GetProjectMembersResult(paged, totalCount, request.Paging);
    }

    #endregion
    
}
    
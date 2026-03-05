using Management.Application.Dtos.Members;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

public sealed record GetSubProjectMembersQuery(
    Guid SubProjectId,
    GetProjectMembersFilter Filter,
    PaginationRequest Paging) : IQuery<GetProjectMembersResult>;

public class GetSubProjectMembersValidator : AbstractValidator<GetSubProjectMembersQuery>
{
    public GetSubProjectMembersValidator()
    {
        RuleFor(x => x.SubProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);
    }
}

public class GetSubProjectMembersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetSubProjectMembersQuery, GetProjectMembersResult>
{
    #region Implementations

    public async Task<GetProjectMembersResult> Handle(
        GetSubProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        // Verify sub project exists
        var subProject = await session.LoadAsync<ProjectEntity>(request.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);
        
        // Load all members of the project
        var allMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == subProject.Id)
            .ToListAsync(cancellationToken);
        if (!allMembers.Any())
            return new GetProjectMembersResult([], 0, request.Paging);
        // Fetch user details (including Keycloak groups) from User service
        var userInfos = await userApiService.GetUsersByIdsAsync(
            userIds: allMembers.Select(x => x.UserId),
            cancellationToken: cancellationToken);

        // Join member records with user info, apply email search
        var joined = allMembers
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
                (string.IsNullOrWhiteSpace(request.Filter.SearchEmail) ||
                 (dto.Email != null && dto.Email.Contains(request.Filter.SearchEmail, StringComparison.OrdinalIgnoreCase)))
                &&
                (string.IsNullOrWhiteSpace(request.Filter.ProjectRole) ||
                 string.Equals(dto.Role, request.Filter.ProjectRole, StringComparison.OrdinalIgnoreCase))
            )
            .OrderBy(dto => dto.JoinedAt)
            .ToList();
        
        var totalCount = joined.Count;
        var paged = joined
            .Skip((request.Paging.PageNumber - 1) * request.Paging.PageSize)
            .Take(request.Paging.PageSize)
            .ToList();

        return new GetProjectMembersResult(paged, totalCount, request.Paging);
    }

    #endregion
    
}

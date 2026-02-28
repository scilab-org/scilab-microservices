using Management.Application.Dtos.Members;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

public record GetProjectMembersQuery(
    Guid ProjectId,
    string? SearchEmail,
    PaginationRequest Paging) : IQuery<GetProjectMembersResult>;

public class GetProjectMembersValidator : AbstractValidator<GetProjectMembersQuery>
{
    public GetProjectMembersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);
    }
}

public class GetProjectMembersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetProjectMembersQuery, GetProjectMembersResult>
{
    #region Implementations

    public async Task<GetProjectMembersResult> Handle(
        GetProjectMembersQuery query,
        CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Load all members of the project
        var allMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == query.ProjectId)
            .ToListAsync(cancellationToken);

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
                string.IsNullOrWhiteSpace(query.SearchEmail) ||
                (dto.Email != null && dto.Email.Contains(query.SearchEmail, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(dto => dto.JoinedAt)
            .ToList();
        
        var totalCount = joined.Count;
        var paged = joined
            .Skip((query.Paging.PageNumber - 1) * query.Paging.PageSize)
            .Take(query.Paging.PageSize)
            .ToList();

        return new GetProjectMembersResult(paged, totalCount, query.Paging);
    }

    #endregion
    
}

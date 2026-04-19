using Management.Application.Dtos.Members;
using Management.Application.Models.Filters;
using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Queries;

[ExcludeFromCodeCoverage]
public record GetProjectMembersQuery(
    Guid ProjectId,
    GetProjectMembersFilter Filter,
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

[ExcludeFromCodeCoverage]
public class GetProjectMembersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetProjectMembersQuery, GetProjectMembersResult>
{
    #region Implementations

    public async Task<GetProjectMembersResult> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(request.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Load all members of the project
        var allMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == request.ProjectId)
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

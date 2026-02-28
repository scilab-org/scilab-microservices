using Management.Application.Models.Results;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Project.Queries;

public sealed record GetAvailableProjectUsersQuery(
    Guid ProjectId,
    string AdminGroupName,
    string? SearchText = null) : IQuery<GetAvailableProjectUsersResult>;

public class GetGetAvailableProjectUsersValidator : AbstractValidator<GetAvailableProjectUsersQuery>
{
    public GetGetAvailableProjectUsersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);
    }
}

public class GetProjectAvailableUsersQueryHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : IQueryHandler<GetAvailableProjectUsersQuery, GetAvailableProjectUsersResult>
{
    #region Implementations

    public async Task<GetAvailableProjectUsersResult> Handle(
        GetAvailableProjectUsersQuery query,
        CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(query.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Get current project member userIds
        var existingMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == query.ProjectId)
            .ToListAsync(cancellationToken);

        var existingMemberUserIds = existingMembers.Select(x => x.UserId);

        // Fetch users not in project and not admin from User service
        var users = await userApiService.GetAvailableProjectUsersAsync(
            existingMemberUserIds: existingMemberUserIds,
            adminGroupName: query.AdminGroupName,
            searchText: query.SearchText,
            cancellationToken: cancellationToken);

        return new GetAvailableProjectUsersResult(users);
    }

    #endregion
}


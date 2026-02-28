using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using Microsoft.AspNetCore.Http;

namespace Management.Application.Features.Member.Commands;

public record AddProjectMembersCommand(Guid ProjectId, AddProjectMembersDto Dto, Guid UserId) : ICommand<List<Guid>>;

public class AddProjectMembersValidator : AbstractValidator<AddProjectMembersCommand>
{
    public AddProjectMembersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.Members)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdsAreRequired);

        RuleForEach(x => x.Dto.Members).ChildRules(member =>
        {
            member.RuleFor(m => m.UserId)
                .NotEmpty()
                .WithMessage(MessageCode.UserIdsAreRequired);
            
        });
    }
}

public class AddProjectMembersCommandHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : ICommandHandler<AddProjectMembersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(AddProjectMembersCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        // Block system-admin group assignment via this endpoint
        if (dto.Members.Any(m =>
            m.GroupName.Contains(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
            throw new(MessageCode.AccessDenied);

        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        // Check current user is ProjectManager in this project
        var isProjectManager = await session.Query<MemberEntity>()
            .AnyAsync(x =>
                    x.ProjectId == command.ProjectId &&
                    x.UserId == command.UserId &&
                    x.ProjectRole == AuthorizeConstants.ProjectManager,
                cancellationToken);

        if (!isProjectManager)
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        // Deduplicate by UserId (keep last entry wins)
        var memberMap = dto.Members
            .Where(m => m.UserId != Guid.Empty)
            .GroupBy(m => m.UserId)
            .ToDictionary(g => g.Key, g => g.Last().GroupName);

        var distinctUserIds = memberMap.Keys.ToList();

        var validUserIds = await userApiService.GetExistingUserIdsAsync(distinctUserIds, cancellationToken);
        if (validUserIds.Count == 0)
            throw new NotFoundException(MessageCode.UserNotFound);

        // Load existing members to prevent duplicates
        var existingMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId)
            .ToListAsync(cancellationToken);

        var existingUserIds = existingMembers.Select(x => x.UserId).ToHashSet();

        // Add only new, valid members
        var createdIds = new List<Guid>();
        var newUserRoles = new List<(Guid UserId, string GroupName)>();
        await session.BeginTransactionAsync(cancellationToken);

        foreach (var userId in validUserIds)
        {
            if (existingUserIds.Contains(userId)) continue;

            var groupName = memberMap.GetValueOrDefault(userId, AuthorizeConstants.User);

            var member = MemberEntity.Create(
                id: Guid.NewGuid(),
                userId: userId,
                projectId: command.ProjectId,
                projectRole: groupName,
                joinedAt: DateTimeOffset.UtcNow);

            session.Store(member);
            createdIds.Add(member.Id);
            newUserRoles.Add((userId, groupName));
        }

        if (!createdIds.Any())
            throw new ClientValidationException(MessageCode.AllMembersAlreadyExist);

        // Assign Keycloak group per user
        foreach (var (userId, groupName) in newUserRoles)
        {
            await userApiService.AssignUserRoleAsync(userId, groupName, cancellationToken);
        }

        await session.SaveChangesAsync(cancellationToken);

        return createdIds;
    }

    #endregion
}

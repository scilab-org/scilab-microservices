using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using Microsoft.AspNetCore.Http;

namespace Management.Application.Features.Member.Commands;

public record AddProjectManagersCommand(Guid ProjectId, AddProjectManagersDto Dto) : ICommand<List<Guid>>;

public class AddProjectManagersValidator : AbstractValidator<AddProjectManagersCommand>
{
    public AddProjectManagersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.UserIds)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdsAreRequired);
    }
}

public class AddProjectManagersCommandHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : ICommandHandler<AddProjectManagersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(AddProjectManagersCommand command, CancellationToken cancellationToken)
    {
        // Only SystemAdmin can add managers
        // if (command.Groups == null ||
        //     !command.Groups.Any(g => g.Equals(AuthorizeConstants.SystemAdmin, StringComparison.OrdinalIgnoreCase)))
        //     throw new NoPermissionException(MessageCode.AccessDenied);

        var dto = command.Dto;

        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);

        // Verify users exist in User service
        var distinctUserIds = dto.UserIds.Distinct().ToList();

        var validUserIds = await userApiService.GetExistingUserIdsAsync(distinctUserIds, cancellationToken);
        if (validUserIds.Count == 0)
            throw new NotFoundException(MessageCode.UserNotFound);

        // Load existing members to prevent duplicates
        var existingMembers = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId)
            .ToListAsync(cancellationToken);

        var existingUserIds = existingMembers.Select(x => x.UserId).ToHashSet();

        // Admin assigns managers — always use ProjectManager Keycloak group
        const string keycloakGroupName = AuthorizeConstants.ProjectManager;

        // Add only new, valid members
        var createdIds = new List<Guid>();
        var newUserIds = new List<Guid>();
        await session.BeginTransactionAsync(cancellationToken);

        foreach (var userId in validUserIds)
        {
            if (existingUserIds.Contains(userId)) continue;

            var member = MemberEntity.Create(
                id: Guid.NewGuid(),
                userId: userId,
                projectId: command.ProjectId,
                projectRole: keycloakGroupName,
                joinedAt: DateTimeOffset.UtcNow);

            session.Store(member);
            createdIds.Add(member.Id);
            newUserIds.Add(userId);
        }

        if (!createdIds.Any())
            throw new ClientValidationException(MessageCode.AllMembersAlreadyExist);

        // Assign Keycloak "manager" group to each new manager via User service
        foreach (var userId in newUserIds)
        {
            await userApiService.AssignUserRoleAsync(userId, keycloakGroupName, cancellationToken);
        }

        await session.SaveChangesAsync(cancellationToken);

        return createdIds;
    }

    #endregion
}

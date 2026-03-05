using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using Microsoft.AspNetCore.Http;

namespace Management.Application.Features.Member.Commands;

public record AddProjectManagersCommand(Guid ProjectId, AddProjectManagersDto Dto) : ICommand<Guid>;
public class AddProjectManagersValidator : AbstractValidator<AddProjectManagersCommand>
{
    public AddProjectManagersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.UserId)
            .NotEmpty()
            .WithMessage(MessageCode.UserIdsAreRequired);
    }
}

public class AddProjectManagersCommandHandler(
    IDocumentSession session,
    IUserApiService userApiService)
    : ICommandHandler<AddProjectManagersCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(AddProjectManagersCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        //Check project already has manager 
        var hasManager = await session.Query<MemberEntity>()
            .AnyAsync(x =>
                    x.ProjectId == command.ProjectId &&
                    x.ProjectRole == AuthorizeConstants.ProjectManager,
                cancellationToken);

        if (hasManager)
            throw new ClientValidationException(MessageCode.ProjectAlreadyHasManager);
        
        // Verify users exist in User service
        var isExist = await userApiService.IsUserExistAsync(dto.UserId, cancellationToken);
        if (!isExist)
            throw new NotFoundException(MessageCode.UserNotFound);

        // Load existing members to prevent duplicates
        var isAlreadyMember = await session.Query<MemberEntity>()
            .AnyAsync(x =>
                    x.ProjectId == command.ProjectId &&
                    x.UserId == dto.UserId,
                cancellationToken);

        if (isAlreadyMember)
            throw new ClientValidationException(MessageCode.MemberAlreadyExists);

        // Admin assigns managers — always use ProjectManager Keycloak group
        const string keycloakGroupName = AuthorizeConstants.ProjectManager;
        
        await session.BeginTransactionAsync(cancellationToken);
        
        var member = MemberEntity.Create(
            id: Guid.NewGuid(),
            userId: dto.UserId,
            projectId: command.ProjectId,
            projectRole: keycloakGroupName,
            joinedAt: DateTimeOffset.UtcNow);

        session.Store(member);
  
        // Assign Keycloak "manager" group to each new manager via User service
        await userApiService.AssignUserRoleAsync(dto.UserId, keycloakGroupName, cancellationToken);


        await session.SaveChangesAsync(cancellationToken);

        return member.Id;
    }

    #endregion
}

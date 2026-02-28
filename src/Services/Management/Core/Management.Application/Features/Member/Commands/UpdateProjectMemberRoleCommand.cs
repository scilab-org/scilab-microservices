using Management.Application.Dtos.Members;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Commands;

public record UpdateProjectMemberRoleCommand(Guid ProjectId, UpdateProjectMemberRoleDto Dto, string UserId) : ICommand<Guid>;

public class UpdateProjectMemberRoleValidator : AbstractValidator<UpdateProjectMemberRoleCommand>
{
    public UpdateProjectMemberRoleValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.MemberId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdsAreRequired);

        RuleFor(x => x.Dto.ProjectRole)
            .NotEmpty()
            .WithMessage(MessageCode.GroupNameIsRequired);
    }
}

public class UpdateProjectMemberRoleCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateProjectMemberRoleCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateProjectMemberRoleCommand command, CancellationToken cancellationToken)
    {
        // Verify project exists
        var project = await session.LoadAsync<ProjectEntity>(command.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        // Check current user is ProjectManager in this project
        var isProjectManager = await session.Query<MemberEntity>()
            .AnyAsync(x =>
                    x.ProjectId == command.ProjectId &&
                    x.UserId == Guid.Parse(command.UserId) &&
                    x.ProjectRole == AuthorizeConstants.ProjectManager,
                cancellationToken);
        if (!isProjectManager)
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        // Load the member
        var member = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId && x.Id == command.Dto.MemberId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(MessageCode.MembersNotFound);

        member.UpdateProjectRole(command.Dto.ProjectRole);

        session.Store(member);
        
        //await userApiService.AssignUserRoleAsync(member.UserId, command.Dto.ProjectRole, cancellationToken);
        
        await session.SaveChangesAsync(cancellationToken);

        return member.Id;
    }

    #endregion
}


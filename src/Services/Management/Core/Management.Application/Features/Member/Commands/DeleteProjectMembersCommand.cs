using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Commands;

public record DeleteProjectMembersCommand(Guid ProjectId, DeleteProjectMembersDto Dto, Guid UserId) : ICommand<List<Guid>>;

public class DeleteProjectMembersValidator : AbstractValidator<DeleteProjectMembersCommand>
{
    public DeleteProjectMembersValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);

        RuleFor(x => x.Dto.MemberIds)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdsAreRequired);
    }
}

public class DeleteProjectMembersCommandHandler(
    IDocumentSession session)
    : ICommandHandler<DeleteProjectMembersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(DeleteProjectMembersCommand command, CancellationToken cancellationToken)
    {
     
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
        
        var memberIdSet = command.Dto.MemberIds.Where(x => x != Guid.Empty).Distinct().ToHashSet();

        if (memberIdSet.Count == 0)
            throw new ClientValidationException(MessageCode.MemberIdsAreRequired);
        
        // Get members to delete from parent project
        var membersToDelete = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId && memberIdSet.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (!membersToDelete.Any())
            throw new NotFoundException(MessageCode.MembersNotFound);
        
        // Get UserIds of members being deleted
        var userIdsToDelete = membersToDelete.Select(m => m.UserId).ToHashSet();
        
        // Check if this project has any subprojects
        var subProjects = await session.Query<ProjectEntity>()
            .Where(x => x.ParentProjectId == command.ProjectId)
            .ToListAsync(cancellationToken);
        
        // Get all members from subprojects that have the same UserIds
        var subProjectMembersToDelete = new List<MemberEntity>();
        if (subProjects.Any())
        {
            var subProjectIds = subProjects.Select(sp => sp.Id).ToList();
            var subProjectMembers = await session.Query<MemberEntity>()
                .Where(x => subProjectIds.Contains(x.ProjectId) && userIdsToDelete.Contains(x.UserId))
                .ToListAsync(cancellationToken);
            subProjectMembersToDelete.AddRange(subProjectMembers);
        }
        
        // Delete all matched members from parent project and subprojects
        await session.BeginTransactionAsync(cancellationToken);

        var deletedIds = new List<Guid>();
        
        // Delete from parent project
        foreach (var member in membersToDelete)
        {
            session.Delete(member);
            deletedIds.Add(member.Id);
        }
        
        // Delete from subprojects
        foreach (var member in subProjectMembersToDelete)
        {
            session.Delete(member);
            deletedIds.Add(member.Id);
        }

        await session.SaveChangesAsync(cancellationToken);

        return deletedIds;
    }

    #endregion
}


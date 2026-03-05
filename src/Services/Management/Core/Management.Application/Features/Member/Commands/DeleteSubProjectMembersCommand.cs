using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;

namespace Management.Application.Features.Member.Commands;

public record DeleteSubProjectMembersCommand(
    Guid SubProjectId,
    DeleteProjectMembersDto Dto, 
    Guid UserId) : ICommand<List<Guid>>;

public class DeleteSubProjectMembersValidator : AbstractValidator<DeleteSubProjectMembersCommand>
{
    public DeleteSubProjectMembersValidator()
    {
        RuleFor(x => x.SubProjectId)
            .NotEmpty()
            .WithMessage(MessageCode.MemberProjectIdIsRequired);
        
        RuleFor(x => x.Dto.MemberIds)
            .NotEmpty()
            .WithMessage(MessageCode.MemberIdsAreRequired);
    }
}

public class DeleteSubProjectMembersCommandHandler(
    IDocumentSession session)
    : ICommandHandler<DeleteSubProjectMembersCommand, List<Guid>>
{
    #region Implementations

    public async Task<List<Guid>> Handle(DeleteSubProjectMembersCommand command, CancellationToken cancellationToken)
    {
        // Verify parent project exists
        var subProject = await session.LoadAsync<ProjectEntity>(command.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.ProjectIsNotExists);
        
        // Check current user is ProjectManager in the PARENT project
        // var isProjectManager = await session.Query<MemberEntity>()
        //     .AnyAsync(x =>
        //             x.ProjectId == command.ProjectId &&
        //             x.UserId == command.UserId &&
        //             x.ProjectRole == AuthorizeConstants.ProjectManager,
        //         cancellationToken);
        // if (!isProjectManager)
        //     throw new NoPermissionException(MessageCode.AccessDenied);
        
        var memberIdSet = command.Dto.MemberIds.Where(x => x != Guid.Empty).Distinct().ToHashSet();

        if (memberIdSet.Count == 0)
            throw new ClientValidationException(MessageCode.MemberIdsAreRequired);
        
        // Delete members from the SUBPROJECT
        var membersToDelete = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == subProject.Id && memberIdSet.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (!membersToDelete.Any())
            throw new NotFoundException(MessageCode.MembersNotFound);
        
        await session.BeginTransactionAsync(cancellationToken);

        var deletedIds = new List<Guid>();
        foreach (var member in membersToDelete)
        {
            session.Delete(member);
            deletedIds.Add(member.Id);
        }

        await session.SaveChangesAsync(cancellationToken);

        return deletedIds;
    }

    #endregion
}


using BuildingBlocks.Authentication.Extensions;
using Management.Application.Dtos.Members;
using Management.Domain.Entities;
using Marten;
using Microsoft.AspNetCore.Http;

namespace Management.Application.Features.Member.Commands;

public record DeleteProjectMembersCommand(Guid ProjectId, DeleteProjectMembersDto Dto, string UserId) : ICommand<List<Guid>>;

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
    IDocumentSession session,
    IHttpContextAccessor httpContextAccessor)
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
                    x.UserId == Guid.Parse(command.UserId) &&
                                              x.ProjectRole == AuthorizeConstants.ProjectManager,
                cancellationToken);
        if (!isProjectManager)
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        var memberIdSet = command.Dto.MemberIds.Where(x => x != Guid.Empty).Distinct().ToHashSet();

        if (memberIdSet.Count == 0)
            throw new (MessageCode.MemberIdsAreRequired);
        
        var membersToDelete = await session.Query<MemberEntity>()
            .Where(x => x.ProjectId == command.ProjectId && memberIdSet.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (!membersToDelete.Any())
            throw new NotFoundException(MessageCode.MembersNotFound);
        // Soft-delete all matched members
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


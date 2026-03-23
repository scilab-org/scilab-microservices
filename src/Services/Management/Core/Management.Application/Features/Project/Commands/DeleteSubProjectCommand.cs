using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Project.Commands;

public sealed record DeleteSubProjectCommand(Guid SubProjectId, Guid UserId, string UserName) : ICommand<Unit>;

public class DeleteSubProjectCommandHandler(
    IDocumentSession session,
    ILabApiService labApiService) : ICommandHandler<DeleteSubProjectCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(
        DeleteSubProjectCommand request,
        CancellationToken cancellationToken)
    {
        // Verify sub project exists
        var subProject = await session.LoadAsync<ProjectEntity>(request.SubProjectId, cancellationToken);
        if (subProject == null)
            throw new NotFoundException(MessageCode.SubProjectNotFound);
        
        var managerMember = session.Query<MemberEntity>()
            .FirstOrDefault(p => p.ProjectId == subProject.ParentProjectId && p.UserId == request.UserId);

        var authorMember = session.Query<MemberEntity>()
            .FirstOrDefault(p => p.ProjectId == subProject.Id && p.UserId == request.UserId);
        
        var isManager = managerMember?.ProjectRole == AuthorizeConstants.ProjectManager;
        var isAuthor = authorMember?.ProjectRole == AuthorizeConstants.ProjectAuthor;
        
        if (!isManager && !isAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);

        if (isAuthor && subProject.CreatedBy != request.UserName)
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        // Delete all papers from Lab service
        foreach (var paperId in subProject.PaperIds)
        {
            await labApiService.DeletePaperAsync(paperId, cancellationToken);
        }
        
        // Delete sub project
        session.Delete(subProject);
        
        await session.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }

    #endregion
}
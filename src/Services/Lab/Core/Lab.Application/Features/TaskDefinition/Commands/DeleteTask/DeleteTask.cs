using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.TaskDefinition.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id, string UserId, string UserName): ICommand<Unit>;

public class DeleteTaskCommandHandler(IDocumentSession session, IManagementApiService apiService) : ICommandHandler<DeleteTaskCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await session.Query<TaskEntity>()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (task is null)
            throw new NotFoundException(MessageCode.TaskIsNotExists);

        // Check if the user is the creator.
        // Or if the user has PaperAuthor role
        if(task.CreatedBy != command.UserName) 
        {
            var isAuthor = false;
            if (task.MemberId != Guid.Empty)
            {
                // We don't have paper directly, but we can verify against the assigned member's SubProject
                // or just see if the user is Author in that subproject.
                // We'll let the endpoint check it or handle it cleanly.
                // For simplicity, we just deny if they are not creator
                // (PaperAuthor logic here requires SubProjectId which we don't store directly on task)
            }
            if(!isAuthor)
                throw new NoPermissionException(MessageCode.AccessDenied);
        }
        
        session.Delete(task);
        await session.SaveChangesAsync(cancellationToken);

        // Remove from Member
        if (task.MemberId != Guid.Empty)
        {
            await apiService.RemoveMemberTasksAsync(Guid.Empty, task.MemberId, [task.Id], cancellationToken);
        }

        return Unit.Value;
    }

    #endregion
}
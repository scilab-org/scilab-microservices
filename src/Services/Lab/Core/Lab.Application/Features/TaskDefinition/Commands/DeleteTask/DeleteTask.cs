using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.TaskDefinition.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id, string UserId, string UserName): ICommand<Unit>;

public class DeleteTaskCommandHandler(IDocumentSession session) : ICommandHandler<DeleteTaskCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await session.Query<TaskEntity>()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
        if (task is null)
            throw new NotFoundException(MessageCode.TaskIsNotExists);

    
        
        var paperContributor = await session.Query<PaperContributorEntity>()
            .Where(x => x.TaskIds.Contains(task.Id))
            .FirstOrDefaultAsync(cancellationToken);
        if(task.CreatedBy != command.UserName && paperContributor == null && paperContributor!.SectionRole.Contains(AuthorizeConstants.PaperAuthor)) 
            throw new NoPermissionException(MessageCode.AccessDenied);
        
        if (paperContributor != null)
        {
            paperContributor.RemoveTasks(task.Id);
            session.Store(paperContributor);
        }
        session.Delete(task);
        
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}
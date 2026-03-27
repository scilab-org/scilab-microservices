using Lab.Application.Dtos.Tasks;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.TaskDefinition.Commands.UpdateTask;

public sealed record UpdateTaskCommand(Guid Id, UpdateTaskDto Dto) : ICommand<Guid>;

public class UpdateTaskCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateTaskCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;
        
        var current = await session.LoadAsync<TaskEntity>(command.Id, cancellationToken);
        if (current == null)
            throw new NotFoundException(MessageCode.TaskIsNotExists);
        
        current.Update(
            name: dto.Name,
            description: dto.Description ?? current.Description,
            assignedToUserName: dto.AssignedToUserName ?? current.AssignedToUserName,
            status: dto.Status,
            startDate: dto.StartDate,
            nextReviewDate: dto.NextReviewDate
        );
        
        if (dto.Status == TaskDefineStatus.Completed)
            current.CompleteDate = DateTimeOffset.UtcNow;
        
        if(current.CompleteDate != null && dto.Status != TaskDefineStatus.Completed)
            current.CompleteDate = null;
        
        session.Store(current);
        
        await session.SaveChangesAsync(cancellationToken);

        return current.Id;
        
    }

    #endregion
}
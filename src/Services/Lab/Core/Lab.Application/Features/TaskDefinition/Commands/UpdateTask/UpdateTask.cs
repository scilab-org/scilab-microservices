using Lab.Application.Dtos.Tasks;
using Lab.Application.Services;
using Lab.Domain.Constants;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Marten;

namespace Lab.Application.Features.TaskDefinition.Commands.UpdateTask;

public sealed record UpdateTaskCommand(Guid Id, UpdateTaskDto Dto, string UserName, string UserId) : ICommand<Guid>;

public class UpdateTaskCommandHandler(IDocumentSession session, IManagementApiService apiService)
    : ICommandHandler<UpdateTaskCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Dto;

        var current = await session.LoadAsync<TaskEntity>(command.Id, cancellationToken);
        if (current == null)
            throw new NotFoundException(MessageCode.TaskIsNotExists);

        // Permission check: author OR the assigned user can update
        var isAssignee = !string.IsNullOrWhiteSpace(current.AssignedToUserName)
                         && current.AssignedToUserName.Equals(command.UserName, StringComparison.OrdinalIgnoreCase);

        if (!isAssignee)
        {
            await EnsureIsAuthorAsync(current, command, cancellationToken);
        }

        current.Update(
            name: dto.Name,
            description: dto.Description ?? current.Description,
            // assignedToUserName: dto.AssignedToUserName ?? current.AssignedToUserName,
            status: dto.Status,
            startDate: dto.StartDate,
            nextReviewDate: dto.NextReviewDate
        );

        if (dto.Status == TaskDefineStatus.Completed)
            current.CompleteDate = DateTimeOffset.UtcNow;

        if (current.CompleteDate != null && dto.Status != TaskDefineStatus.Completed)
            current.CompleteDate = null;

        session.Store(current);

        await session.SaveChangesAsync(cancellationToken);

        return current.Id;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Verifies the current user holds the PaperAuthor role in the project that owns this task.
    /// Throws <see cref="NoPermissionException"/> if the check fails.
    /// </summary>
    private async Task EnsureIsAuthorAsync(
        TaskEntity task, UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var taskMemberInfo = await apiService.GetMemberByIdAsync(task.MemberId, cancellationToken);
        if (taskMemberInfo == null)
            throw new NoPermissionException(MessageCode.AccessDenied);

        var role = await apiService.GetMyProjectRoleAsync(taskMemberInfo.ProjectId, cancellationToken);
        if (string.IsNullOrWhiteSpace(role) || role != AuthorizeConstants.PaperAuthor)
            throw new NoPermissionException(MessageCode.AccessDenied);
    }

    #endregion
}
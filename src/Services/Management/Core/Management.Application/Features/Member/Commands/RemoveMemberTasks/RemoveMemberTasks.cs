using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Member.Commands.RemoveMemberTasks;

public sealed record RemoveMemberTasksCommand(Guid MemberId, List<Guid> TaskIds) : ICommand<Unit>;

public class RemoveMemberTasksCommandHandler(IDocumentSession session) : ICommandHandler<RemoveMemberTasksCommand, Unit>
{
    public async Task<Unit> Handle(RemoveMemberTasksCommand request, CancellationToken cancellationToken)
    {
        var member = await session.LoadAsync<MemberEntity>(request.MemberId, cancellationToken);
        if (member == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.MemberId.ToString());

        foreach (var taskId in request.TaskIds)
        {
            member.RemoveTasks(taskId);
        }

        session.Store(member);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

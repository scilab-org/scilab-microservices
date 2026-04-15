using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Member.Commands.AddMemberTasks;

public sealed record AddMemberTasksCommand(Guid MemberId, List<Guid> TaskIds) : ICommand<Unit>;

public class AddMemberTasksCommandHandler(IDocumentSession session) : ICommandHandler<AddMemberTasksCommand, Unit>
{
    public async Task<Unit> Handle(AddMemberTasksCommand request, CancellationToken cancellationToken)
    {
        var member = await session.LoadAsync<MemberEntity>(request.MemberId, cancellationToken);
        if (member == null)
            throw new NotFoundException(MessageCode.MemberNotFound, request.MemberId.ToString());

        foreach (var taskId in request.TaskIds)
        {
            if (!member.TaskIds.Contains(taskId))
            {
                member.AddTasks(taskId);
            }
        }

        session.Store(member);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Comment.Commands.UpdateComment;

public sealed record UpdateCommentCommand(Guid Id, string Content, string UserName) : ICommand<Guid>;


public class UpdateTemplateCommandCommandHandler(IDocumentSession session)
    : ICommandHandler<UpdateCommentCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var current = await session.Query<CommentEntity>()
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.UserName == command.UserName, cancellationToken);
        if (current == null)
            throw new NotFoundException(MessageCode.CommentNotFound);
        
        current.Update(
            content: command.Content
        );
        
        session.Store(current);
        await session.SaveChangesAsync(cancellationToken);

        return current.Id;
        
    }

    #endregion
}
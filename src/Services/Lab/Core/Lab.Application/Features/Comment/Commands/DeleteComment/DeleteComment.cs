using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Comment.Commands.DeleteComment;

public sealed record DeleteCommentCommand(Guid Id, Guid MarkSectionId, string UserName) : ICommand<Unit>;

public class DeleteCommentCommandHandler(IDocumentSession session)
    : ICommandHandler<DeleteCommentCommand, Unit>
{
    #region Implementations

    public async Task<Unit> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        
        var comment = await session.Query<CommentEntity>()
            .FirstOrDefaultAsync(x => x.Id == command.Id && x.UserName == command.UserName, cancellationToken);
        if (comment is null)
            throw new NotFoundException(MessageCode.CommentNotFound);

        var section = await session.LoadAsync<SectionEntity>(command.MarkSectionId, cancellationToken);
        if (section != null)
        {
            section.RemoveComment(comment.Id);
            session.Update(section);
        }

        session.Delete(comment);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    #endregion
}


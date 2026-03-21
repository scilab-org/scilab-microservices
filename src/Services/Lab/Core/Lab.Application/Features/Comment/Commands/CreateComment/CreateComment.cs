using Lab.Application.Dtos.Comments;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Comment.Commands.CreateComment;

public sealed record CreateCommentCommand(string UserName, CreateCommentDto Dto) : ICommand<Guid>;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Dto.Content)
            .NotEmpty().WithMessage(MessageCode.CommentContentIsRequired);
    }
}

public class CreateCommentCommandHandler(IDocumentSession session) : IRequestHandler<CreateCommentCommand, Guid>

{
    #region Implementations

    public async Task<Guid> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        await session.BeginTransactionAsync(cancellationToken);
        
        var entity = CommentEntity.Create(
            id: Guid.NewGuid(),
            sectionId: dto.SectionId,
            content: dto.Content,
            userName: request.UserName);
        
        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }

    #endregion
}
using AutoMapper;
using Lab.Application.Dtos.Comments;
using Lab.Application.Models.Results;
using Lab.Domain.Entities;
using Marten;

namespace Lab.Application.Features.Comment.Queries.GetCommentsBySectionId;

public sealed record GetCommentsBySectionIdQuery(Guid SectionId) : IQuery<GetCommentsBySectionIdResult>;

public class GetCommentsBySectionIdQueryHandler(IDocumentSession session, IMapper mapper)
    : IQueryHandler<GetCommentsBySectionIdQuery, GetCommentsBySectionIdResult>
{
    #region Implementations

    public async Task<GetCommentsBySectionIdResult> Handle(GetCommentsBySectionIdQuery request, CancellationToken cancellationToken)
    {
        var comments = await session.Query<CommentEntity>()
            .Where(x => x.SectionId == request.SectionId)
            .OrderByDescending(x => x.CreatedOnUtc)
            .ToListAsync(cancellationToken);

        var commentDtos = mapper.Map<List<CommentDto>>(comments);

        return new GetCommentsBySectionIdResult(commentDtos);
    }

    #endregion
}
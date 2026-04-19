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
        var section = await session.LoadAsync<SectionEntity>(request.SectionId, cancellationToken);
        if (section == null || section.CommentIds == null || !section.CommentIds.Any())
        {
            return new GetCommentsBySectionIdResult(new List<CommentDto>());
        }

        var comments = await session.LoadManyAsync<CommentEntity>(cancellationToken, section.CommentIds.ToArray());
        
        var sortedComments = comments.OrderByDescending(x => x.CreatedOnUtc).ToList();

        var commentDtos = mapper.Map<List<CommentDto>>(sortedComments);

        return new GetCommentsBySectionIdResult(commentDtos);
    }

    #endregion
}
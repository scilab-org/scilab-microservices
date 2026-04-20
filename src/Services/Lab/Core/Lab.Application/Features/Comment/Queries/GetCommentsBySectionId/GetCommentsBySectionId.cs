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
        var mainSection = await session.LoadAsync<SectionEntity>(request.SectionId, cancellationToken);
        if (mainSection == null || mainSection.CommentIds == null || !mainSection.CommentIds.Any())
        {
            return new GetCommentsBySectionIdResult(new List<CommentDto>());
        }

        var comments = await session.LoadManyAsync<CommentEntity>(cancellationToken, mainSection.CommentIds.ToArray());
        
        var sortedComments = comments.OrderByDescending(x => x.CreatedOnUtc).ToList();

        var sections = await session.LoadManyAsync<SectionEntity>(cancellationToken, sortedComments.Select(x => x.SectionId).Distinct().ToArray());

        var commentDtos = mapper.Map<List<CommentDto>>(sortedComments);
        foreach (var commentDto in commentDtos)
        {
            var commentSection = sections.FirstOrDefault(x => x.Id == commentDto.SectionId);
            commentDto.SectionContent = commentSection?.Content;
        }

        return new GetCommentsBySectionIdResult(commentDtos);
    }

    #endregion
}
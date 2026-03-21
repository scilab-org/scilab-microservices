using Lab.Application.Dtos.Comments;

namespace Lab.Application.Models.Results;

public class GetCommentsBySectionIdResult
{
    public List<CommentDto> Items { get; init; }

    public GetCommentsBySectionIdResult(List<CommentDto> items)
    {
        Items = items;
    }
}
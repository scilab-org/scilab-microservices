using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Comments;

public class CommentInfoDto: DtoId<Guid>
{
    public Guid SectionId { get; set; }
    public string Content { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? ReplyToUserName { get; set; }
}
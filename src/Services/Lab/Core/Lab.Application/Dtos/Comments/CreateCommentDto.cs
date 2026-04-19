namespace Lab.Application.Dtos.Comments;

public class CreateCommentDto
{
    public Guid SectionId { get; set; }
    public string Content { get; set; } = null!;
    public Guid MarkSectionId { get; set; }
    public string? RepliedToUserName { get; set; }
    
}
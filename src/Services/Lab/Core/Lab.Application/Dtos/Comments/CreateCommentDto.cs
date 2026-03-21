namespace Lab.Application.Dtos.Comments;

public class CreateCommentDto
{
    public Guid SectionId { get; set; }
    public string Content { get; set; } = null!;
}
namespace Lab.Application.Dtos.Papers;

public class UpdateCombinePaperDto
{
    public string Content { get; set; } = null!;
    public Guid ProjectId { get; set; }
}
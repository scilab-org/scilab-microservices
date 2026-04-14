namespace Lab.Application.Dtos.Papers;

public class CreatePaperCombineDto
{
    public string? Content { get; set; } = null;
    public Guid ProjectId { get; set; }
}
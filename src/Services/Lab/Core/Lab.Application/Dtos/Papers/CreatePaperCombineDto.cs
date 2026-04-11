namespace Lab.Application.Dtos.Papers;

public class CreatePaperCombineDto
{
    public bool IsPreview { get; set; } = true;
    public string? Content { get; set; } = null;
    public Guid ProjectId { get; set; }
}
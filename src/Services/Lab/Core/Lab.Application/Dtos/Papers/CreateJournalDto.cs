namespace Lab.Application.Dtos.Papers;

public class CreateJournalDto
{
    public string Name { get; set; } = null!;
    public string StyleName { get; set; } = null!;
    public string StyleDescription { get; set; } = null!;
    public string StyleRule { get; set; } = null!;
}
namespace Lab.Application.Dtos.Papers;

public class CreateJournalDto
{
    public Guid JournalId { get; init; }
    public string StyleName { get; set; } = null!;
    public string StyleDescription { get; set; } = null!;
    public string StyleRule { get; set; } = null!;
}
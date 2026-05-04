namespace Lab.Application.Dtos.CheckLists;

public class CheckListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Rule { get; set; } = null!;
    public int Weight { get; set; }
}

namespace Lab.Domain.Models;

public class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Rule { get; set; } = null!;
    public int Weight { get; set; }
}
namespace Lab.Domain.Models;

public class Reference
{
    public Guid PaperId { get; set; }
    public List<Guid> SectionIds { get; set; } = new();
}
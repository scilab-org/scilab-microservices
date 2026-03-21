using Lab.Domain.Abstractions;
using Lab.Domain.Models;

namespace Lab.Domain.Entities;

public class JournalEntity : Entity<Guid>
{
    public string Name { get; set; } = null!;
    public List<Style> Styles { get; set; } = [];
}
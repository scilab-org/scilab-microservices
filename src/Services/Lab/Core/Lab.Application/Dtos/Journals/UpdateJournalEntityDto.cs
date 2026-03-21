using Lab.Domain.Models;

namespace Lab.Application.Dtos.Journals;

/// <summary>
/// DTO for updating an existing Journal Entity
/// </summary>
public class UpdateJournalEntityDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public List<Style>? Styles { get; set; }
}
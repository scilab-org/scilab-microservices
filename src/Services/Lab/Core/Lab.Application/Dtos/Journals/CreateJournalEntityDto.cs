using Lab.Domain.Models;

namespace Lab.Application.Dtos.Journals;

/// <summary>
/// DTO for creating a new Journal Entity
/// Note: Different from Papers.CreateJournalDto which is for journal styles within a paper
/// </summary>
public class CreateJournalEntityDto
{
    public string Name { get; set; } = null!;
    public List<Style>? Styles { get; set; }
}
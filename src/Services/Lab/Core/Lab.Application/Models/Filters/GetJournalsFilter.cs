using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public record class GetJournalsFilter
{
    public string? Name { get; set; }
    public string? ISSN { get; set; }
    public ConferenceJournalType? Type { get; set; }
    public string? Ranking { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? PaperId { get; set; }
    public bool? IsDeleted { get; set; } = false;
}
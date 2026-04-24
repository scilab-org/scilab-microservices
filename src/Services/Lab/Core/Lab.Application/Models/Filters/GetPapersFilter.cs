using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public record class GetPapersFilter
{
    public string? Title { get; set; }
    public string? Template { get; set; }
    public string? Context { get; set; }
    public string? Abstract { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? ResearchAim { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public PaperStatus? Status { get; set; }
    public bool? IsDeleted { get; set; } = false;
}

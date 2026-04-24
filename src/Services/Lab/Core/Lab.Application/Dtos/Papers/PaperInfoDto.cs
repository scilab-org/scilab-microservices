using Lab.Application.Dtos.Abstractions;
using Lab.Application.Dtos.GapTypes;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public  class PaperInfoDto : DtoId<Guid>
{
    public string Title { get; set; } = null!;
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public string? Context { get; set; }
    public string? Abstract { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? ResearchAim { get; set; }
    public string? Rule { get; set; }
    public List<GapTypeInfoDto> GapTypes { get; set; } = new();
    public Guid? ConferenceJournalId { get; set; }
    public string? ConferenceJournalName { get; set; }
    public DateTimeOffset? ConferenceJournalStartAt { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
    public Guid? SubProjectId { get; set; }
    public PaperStatus? Status { get; set; }
    public SubmissionStatus? SubmissionStatus { get; set; }
    public List<string> TagNames { get; set; } = new();
    public List<PaperVersionInfo> Versions { get; set; } = new();
    public List<PaperReferenceInfo> References { get; set; } = new();
}
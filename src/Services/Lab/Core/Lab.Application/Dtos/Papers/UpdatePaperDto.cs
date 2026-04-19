using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class UpdatePaperDto
{
    public string Context { get; init; } = null!;
    public string Abstract { get; init; } = null!;
    public string ResearchGap { get; init; } = null!;
    public string GapType { get; init; } = null!;
    public string? MainContribution { get; init; } = null!;
    public string? ResearchAim { get; init; } = null!;
    public DateTimeOffset? ConferenceJournalStartAt { get; init; }
    public DateTimeOffset? ConferenceJournalEndAt { get; init; }
}
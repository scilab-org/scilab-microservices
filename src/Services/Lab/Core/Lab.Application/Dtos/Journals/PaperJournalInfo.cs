namespace Lab.Application.Dtos.Journals;

public class PaperJournalInfo
{
    public Guid Id { get; init; }
    public string? Title { get; init; } = null!;
    public DateTimeOffset? ConferenceJournalStartAt { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
}
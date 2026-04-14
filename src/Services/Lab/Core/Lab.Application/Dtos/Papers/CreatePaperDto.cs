using Lab.Application.Dtos.Sections;
using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class CreatePaperDto
{
    #region Fields, Properties and Indexers

    public Guid ProjectId { get; init; }
    public string Title { get; init; } = null!;
    public string? Template { get; init; }
    public string Context { get; init; } = null!;
    public string Abstract { get; init; } = null!;
    public string ResearchGap { get; init; } = null!;
    public string GapType { get; init; } = null!;
    public string? MainContribution { get; init; } = null!;
    public string? ResearchAim { get; init; } = null!;
    public PaperStatus? Status { get; init; } = PaperStatus.Draft;
    public string ConferenceJournalName { get; init; } = null!;
    public Guid ConferenceJournalId { get; init; }
    public List<CreateSectionDto>? Sections { get; init; }

    #endregion
}
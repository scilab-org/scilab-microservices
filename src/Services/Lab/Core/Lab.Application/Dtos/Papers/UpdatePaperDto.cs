using Lab.Domain.Enums;

namespace Lab.Application.Dtos.Papers;

public class UpdatePaperDto
{
    public string Context { get; init; } = null!;
    public string Abstract { get; init; } = null!;
    public string ResearchGap { get; init; } = null!;
    public string GapType { get; init; } = null!;
    public string? MainContribution { get; init; } = null!;
    public PaperStatus? Status { get; init; } = PaperStatus.Draft;
    public CreateJournalDto Journal { get; init; } = null!;
}
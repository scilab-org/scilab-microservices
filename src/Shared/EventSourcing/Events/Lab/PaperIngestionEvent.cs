using EventSourcing.Events;
namespace EventSourcing.Events.Lab;

public sealed record PaperIngestionEvent : IntegrationEvent
{
    #region Fields, Properties and Indexers

    public Guid PaperId { get; init; }

    public string PaperName { get; init; } = string.Empty;

    public string ReferenceKey { get; init; } = string.Empty;

    public string Authors { get; init; } = string.Empty;

    public string Publisher { get; init; } = string.Empty;

    public string JournalName { get; init; } = string.Empty;

    public string Volume { get; init; } = string.Empty;

    public string Pages { get; init; } = string.Empty;

    public string Doi { get; init; } = string.Empty;

    /// <summary>Formatted as "MMMM yyyy", e.g. "May 2015". Empty when unknown.</summary>
    public string PublicationMonthYear { get; init; } = string.Empty;

    public string ParsedText { get; init; } = string.Empty;

    #endregion
}
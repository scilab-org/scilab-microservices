using EventSourcing.Events;
namespace EventSourcing.Events.Lab;

public sealed record PaperIngestionEvent : IntegrationEvent
{
    #region Fields, Properties and Indexers

    public Guid PaperId { get; init; }

    public string PaperName { get; init; } = default!;
    
    public string? ReferenceKey { get; init; }

    public string? Authors { get; init; }

    public string? Publisher { get; init; }

    public string? JournalName { get; init; }

    public string? Volume { get; init; }

    public string? Pages { get; init; }

    public string? Doi { get; init; }

    /// <summary>Formatted as "MMMM yyyy", e.g. "May 2015". Null when publication date is unknown.</summary>
    public string? PublicationMonthYear { get; init; }

    public string ParsedText { get; init; } = default!;

    #endregion
}
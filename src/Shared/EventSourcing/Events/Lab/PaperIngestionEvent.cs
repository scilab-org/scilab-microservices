using EventSourcing.Events;
namespace EventSourcing.Events.Lab;

public sealed record PaperIngestionEvent : IntegrationEvent
{
    #region Fields, Properties and Indexers

    public Guid PaperId { get; init; }

    public string PaperName { get; init; } = default!;
    
    public string ParsedText { get; init; } = default!;

    #endregion
}
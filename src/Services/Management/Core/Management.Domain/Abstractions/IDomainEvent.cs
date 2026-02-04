namespace Management.Domain.Abstractions;

public interface IDomainEvent
{
    #region Fields, Properties and Indexers

    Guid EventId => Guid.NewGuid();

    public DateTimeOffset OccurredOn => DateTime.Now;

    public string EventType => GetType()?.AssemblyQualifiedName ?? string.Empty;

    #endregion
}
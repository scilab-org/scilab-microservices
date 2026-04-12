#region using

using MediatR;

#endregion

using System.Diagnostics.CodeAnalysis;

namespace User.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    #region Fields, Properties and Indexers

    [ExcludeFromCodeCoverage]
    Guid EventId => Guid.NewGuid();

    [ExcludeFromCodeCoverage]
    public DateTimeOffset OccurredOn => DateTime.Now;

    [ExcludeFromCodeCoverage]
    public string EventType => GetType()?.AssemblyQualifiedName ?? string.Empty;

    #endregion

}

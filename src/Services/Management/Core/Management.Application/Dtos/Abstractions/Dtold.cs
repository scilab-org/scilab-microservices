using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Abstractions;

[ExcludeFromCodeCoverage]
public class DtoId<T> : IDtoId<T>
{
    #region Fields, Properties and Indexers

    public T Id { get; init; } = default!;

    #endregion
}
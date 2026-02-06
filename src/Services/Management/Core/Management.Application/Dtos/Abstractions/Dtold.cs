namespace Management.Application.Dtos.Abstractions;

public class DtoId<T> : IDtoId<T>
{
    #region Fields, Properties and Indexers

    public T Id { get; init; } = default!;

    #endregion
}
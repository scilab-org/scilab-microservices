namespace Management.Application.Dtos.Abstractions;

public interface IDtoId<T>
{
    T Id { get; init; }
}
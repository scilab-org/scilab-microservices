using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public sealed class KeywordEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;

    #endregion

    #region Factories

    public static KeywordEntity Create(Guid id,
        string name)
    {
        return new KeywordEntity()
        {
            Id = id,
            Name = name,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? name)
    {
        Name = name ?? Name;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
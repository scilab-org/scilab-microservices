using Lab.Domain.Abstractions;
using Lab.Domain.Models;

namespace Lab.Domain.Entities;

public class JournalEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public List<Style> Styles { get; set; } = [];

    #endregion

    #region Factories

    public static JournalEntity Create(Guid id, string name, List<Style>? styles = null)
    {
        return new JournalEntity()
        {
            Id = id,
            Name = name,
            Styles = styles ?? [],
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? name, List<Style>? styles = null)
    {
        Name = name ?? Name;
        if (styles != null)
            Styles = styles;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
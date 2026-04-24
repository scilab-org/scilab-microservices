using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public class DomainEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    
    #endregion
    #region Factories
    
    public static DomainEntity Create(Guid id,
        string name)
    {
        return new DomainEntity()
        {
            Id = id,
            Name = name,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    #endregion
    
    #region Methods
    public void Update(string name)
    {
        Name = name;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
 
    #endregion
}
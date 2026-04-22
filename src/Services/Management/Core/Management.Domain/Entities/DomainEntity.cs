using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public class DomainEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    
    #endregion
    #region Factories
    
    public static DomainEntity Create(Guid id,
        string name,
        string? description = null)
    {
        return new DomainEntity()
        {
            Id = id,
            Name = name,
            Description = description,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    #endregion
    
    #region Methods
    public void Update(string name,
        string? description = null)
    {
        Name = name;
        Description = description;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
 
    #endregion
}
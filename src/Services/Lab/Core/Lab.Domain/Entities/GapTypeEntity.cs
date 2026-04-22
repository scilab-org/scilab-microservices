using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class GapTypeEntity: Entity<Guid>
{
    public string Name { get; set; } = null!;
    
    #region Factories
    
    public static GapTypeEntity Create(Guid id, 
        string name)
    {
        return new GapTypeEntity()
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
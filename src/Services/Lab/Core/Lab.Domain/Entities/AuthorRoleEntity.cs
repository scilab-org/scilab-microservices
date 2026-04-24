using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class AuthorRoleEntity: Entity<Guid>
{ 
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    #endregion
    
    #region Factories
    
    public static AuthorRoleEntity Create(Guid id, 
        string name, 
        string? description = null)
    {
        return new AuthorRoleEntity()
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
        Description = description ?? Description;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    #endregion
}
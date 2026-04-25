using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public class AffiliationEntity: Entity<Guid>
{
    #region Fields, Properties and Indexers
    
    public string Name { get; set; } = null!;
    public string? ShortName { get; set; }
    public string RorId { get; set; } = null!; // Research Organization Registry (ROR) ID
    public string? RorUrl { get; set; } = null!;
    
    #endregion
    
    #region Factories
    
    public static AffiliationEntity Create(Guid id,
        string name,
        string? shortName,
        string rorId,
        string? rorUrl)
    {
        return new AffiliationEntity()
        {
            Id = id,
            Name = name,
            ShortName = shortName,
            RorId = rorId,
            RorUrl = rorUrl,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    
    #endregion
    
    #region Methods
    
    public void Update(string? name = null,
        string? shortName = null,
        string? rorId = null,
        string? rorUrl = null)
    {
        Name = name ?? Name;
        ShortName = shortName ?? ShortName;
        RorId = rorId ?? RorId;
        RorUrl = rorUrl ?? RorUrl;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    #endregion
}
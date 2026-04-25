using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public class UserAffiliationEntity: Entity<Guid>
{
    #region Fields, Properties and Indexers
    
    public Guid UserId { get; set; }
    public Guid AffiliationId { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public int? AffiliationStartYear { get; set; }
    public int? AffiliationEndYear { get; set; }
    
    #endregion
    
    #region Factories
     
    
    public static UserAffiliationEntity Create(Guid id,
        Guid userId,
        Guid affiliationId,
        string? department,
        string? position,
        int? affiliationStartYear,
        int? affiliationEndYear)
    {
        return new UserAffiliationEntity()
        {
            Id = id,
            UserId = userId,
            AffiliationId = affiliationId,
            Department = department,
            Position = position,
            AffiliationStartYear = affiliationStartYear,
            AffiliationEndYear = affiliationEndYear,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    
    #endregion
    
    #region Methods
    
    public void Update(Guid? userId = null,
        Guid? affiliationId = null,
        string? department = null,
        string? position = null,
        int? affiliationStartYear = null,
        int? affiliationEndYear = null)
    {
        UserId = userId ?? UserId;
        AffiliationId = affiliationId ?? AffiliationId;
        Department = department ?? Department;
        Position = position ?? Position;
        AffiliationStartYear = affiliationStartYear ?? AffiliationStartYear;
        AffiliationEndYear = affiliationEndYear ?? AffiliationEndYear;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
        
    #endregion
    
}
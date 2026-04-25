using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class PaperAuthorEntity: Entity<Guid>
{
    #region Fields, Properties and Indexers
    public string Name { get; set; } = null!;
    public string? OcrId { get; set; }
    public string Email { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid AuthorRoleId { get; set; }
    public Guid MemberId { get; set; }
    public Guid AffiliationId { get; set; }
    public string AffiliationName { get; set; } = null!;
    
    #endregion

    #region Factories
    
    public static PaperAuthorEntity Create(Guid id, 
        string name, 
        string? ocrId, 
        string email, 
        Guid paperId, 
        Guid authorRoleId, 
        Guid memberId,
        Guid affiliationId,
        string affiliationName)
    {
        return new PaperAuthorEntity()
        {
            Id = id,
            Name = name,
            OcrId = ocrId,
            Email = email,
            PaperId = paperId,
            AuthorRoleId = authorRoleId,
            MemberId = memberId,
            AffiliationId = affiliationId,
            AffiliationName = affiliationName,
            CreatedOnUtc = DateTimeOffset.Now,
            LastModifiedOnUtc = DateTimeOffset.Now
        };
    }
    
    public void Update(string? name = null, 
        string? ocrId = null, 
        string? email = null,
        Guid? paperId = null,
        Guid? authorRoleId = null, 
        Guid? memberId = null,
        Guid? affiliationId = null,
        string? affiliationName = null)
    {
        Name = name ?? Name;
        OcrId = ocrId ?? OcrId;
        Email = email ?? Email;
        PaperId = paperId ?? PaperId;
        AuthorRoleId = authorRoleId ?? AuthorRoleId;
        MemberId = memberId ?? MemberId;
        AffiliationId = affiliationId ?? AffiliationId;
        AffiliationName = affiliationName ?? AffiliationName;
        LastModifiedOnUtc = DateTimeOffset.Now;
    }
    #endregion
}
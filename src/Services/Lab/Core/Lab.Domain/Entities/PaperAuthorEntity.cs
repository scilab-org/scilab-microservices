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
    
    #endregion

    #region Factories
    
    public static PaperAuthorEntity Create(Guid id, 
        string name, 
        string? ocrId, 
        string email, 
        Guid paperId, 
        Guid authorRoleId, 
        Guid memberId)
    {
        return new PaperAuthorEntity()
        {
            Id = id,
            Name = name,
            OcrId = ocrId,
            Email = email,
            PaperId = paperId,
            AuthorRoleId = authorRoleId,
            MemberId = memberId
        };
    }
    
    public void Update(string? name = null, 
        string? ocrId = null, 
        string? email = null,
        Guid? paperId = null,
        Guid? authorRoleId = null, 
        Guid? memberId = null)
    {
        Name = name ?? Name;
        OcrId = ocrId ?? OcrId;
        Email = email ?? Email;
        PaperId = paperId ?? PaperId;
        AuthorRoleId = authorRoleId ?? AuthorRoleId;
        MemberId = memberId ?? MemberId;
    }
    #endregion
}
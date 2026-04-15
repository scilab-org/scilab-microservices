using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class PaperContributorEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string SectionRole { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Guid? SectionId { get; set; }
    public Guid MemberId { get; set; }
    public Guid MarkSectionId { get; set; }

    #endregion

    #region Factories

    public static PaperContributorEntity Create(Guid id,
        string sectionRole,
        Guid paperId,
        Guid? sectionId,
        Guid memberId,
        Guid markSectionId)
    {
        return new PaperContributorEntity()
        {
            Id = id,
            SectionRole = sectionRole,
            PaperId = paperId,
            SectionId = sectionId,
            MemberId = memberId,
            MarkSectionId = markSectionId,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods
    
    public void Update(
        Guid? sectionId = null,
        Guid? memberId = null,
        Guid? markSectionId = null,
        string? sectionRole = null)
    {
        SectionRole = sectionRole ?? SectionRole;
        SectionId = sectionId ?? SectionId;
        MemberId = memberId ?? MemberId;
        MarkSectionId = markSectionId ?? MarkSectionId;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    #endregion
    
}
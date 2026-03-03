using Lab.Domain.Abstractions;

namespace Lab.Domain.Entities;

public class SectionEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string? Content { get; set; } = null!;
    public string? Title { get; set; }
    public string? SectionSumary { get; set; }
    public float DisplayOrder { get; set; }
    public bool? Numbered { get; set; } = true;
    public string? FilePath { get; set; }
    public Guid? ParentSectionId { get; set; }
    public Guid? PreviousVersionSectionId { get; set; }
    public Guid? NextVersionSectionId { get; set; }
    public Guid PaperId { get; set; }

    #endregion

    #region Factories

    public static SectionEntity Create(Guid id,
        string? content,
        Guid paperId,
        float displayOrder,
        bool? numbered = true,
        string? title = null,
        string? sectionSumary = null,
        Guid? parentSectionId = null,
        Guid? previousVersionSectionId = null,
        Guid? nextVersionSectionId = null
    )
    {
        return new SectionEntity()
        {
            Id = id,
            Content = content,
            PaperId = paperId,
            Title = title,
            SectionSumary = sectionSumary,
            DisplayOrder = displayOrder,
            Numbered = numbered,
            ParentSectionId = parentSectionId,
            PreviousVersionSectionId = previousVersionSectionId,
            NextVersionSectionId = nextVersionSectionId,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update()
    {
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateFilePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        FilePath = url;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
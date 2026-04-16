using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class SectionEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string? Content { get; set; } = null!;
    public string? Title { get; set; }
    public string? SectionSumary { get; set; }
    public string? Description { get; set; }
    public SectionStatus Status { get; set; }
    public string? MainIdea { get; set; }
    public string? Rule { get; set; }
    public string? PaperRule { get; set; }
    public string? ProjectRule { get; set; }
    public string? SectionRule { get; set; }
    public float DisplayOrder { get; set; }
    public bool? IsMainSection { get; set; } = false;
    public bool? IsOldMainSection { get; set; } = false;
    public string? Version { get; set; }
    public Guid? PreviousVersionSectionId { get; set; }
    public Guid? NextVersionSectionId { get; set; }
    public Guid PaperId { get; set; }
    public List<string>? Files { get; set; }
    public List<Guid>? References { get; set; }
    public List<string>? Packages { get; set; }

    #endregion

    #region Factories

    public static SectionEntity Create(Guid id,
        string? content,
        Guid paperId,
        float displayOrder,
        SectionStatus status,
        bool? isMainSection = false,
        bool? isOldMainSection = false,
        string? version = null,
        string? title = null,
        string? sectionSumary = null,
        string? description = null,
        string? mainIdea = null,
        string? rule = null,
        Guid? previousVersionSectionId = null,
        Guid? nextVersionSectionId = null,
        string? createdBy = null,
        List<Guid>? references = null,
        string? paperRule = null,
        string? projectRule = null,
        string? sectionRule = null,
        List<string>? files = null,
        List<string>? packages = null)
    {
        return new SectionEntity()
        {
            Id = id,
            Content = content,
            PaperId = paperId,
            Title = title,
            Status = status,
            Version = version,
            SectionSumary = sectionSumary,
            Description = description,
            MainIdea = mainIdea,
            Rule = rule,
            PaperRule = paperRule,
            ProjectRule = projectRule,
            SectionRule = sectionRule,
            DisplayOrder = displayOrder,
            IsMainSection = isMainSection,
            IsOldMainSection = isOldMainSection,
            PreviousVersionSectionId = previousVersionSectionId,
            NextVersionSectionId = nextVersionSectionId,
            CreatedBy = createdBy,
            Files = files,
            References = references,
            Packages = packages,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? content = null,
        string? title = null,
        float? displayOrder = null,
        string? sectionSumary = null,
        string? description = null,
        SectionStatus? status = null,
        string? mainIdea = null,
        string? rule = null,
        bool? isMainSection = null,
        bool? isOldMainSection = null,
        string? version = null,
        Guid? previousVersionSectionId = null,
        Guid? nextVersionSectionId = null,
        Guid? paperId = null,
        List<Guid>? references = null,
        string? paperRule = null,
        string? projectRule = null,
        string? sectionRule = null,
        List<string>? files = null,
        List<string>? packages = null,
        string? lastModifiedBy = null!)
    {
        Content = content ?? Content;
        Title = title ?? Title;
        DisplayOrder = displayOrder ?? DisplayOrder;
        SectionSumary = sectionSumary ?? SectionSumary;
        Description = description ?? Description;
        Status = status ?? Status;
        MainIdea = mainIdea ?? MainIdea;
        Rule = rule ?? Rule;
        PaperRule = paperRule ?? PaperRule;
        ProjectRule = projectRule ?? ProjectRule;
        SectionRule = sectionRule ?? SectionRule;
        IsMainSection = isMainSection ?? IsMainSection;
        IsOldMainSection = isOldMainSection ?? IsOldMainSection;
        Version = version ?? Version;
        PreviousVersionSectionId = previousVersionSectionId ?? PreviousVersionSectionId;
        NextVersionSectionId = nextVersionSectionId ?? NextVersionSectionId;
        PaperId = paperId ?? PaperId;
        Files = files ?? Files;
        References = references ?? References;
        Packages = packages ?? Packages;
        LastModifiedBy = lastModifiedBy ?? LastModifiedBy;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateFilePath(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        Files ??= new List<string>();
        Files.Add(url);
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
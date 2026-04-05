using Lab.Domain.Abstractions;
using Lab.Domain.Enums;
using Lab.Domain.Models;

namespace Lab.Domain.Entities;

public sealed class PaperEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Title { get; set; } = null!;
    public string? Template { get; set; }
    public string? FilePath { get; set; }
    public string? Context { get; set; }
    public string? Abstract { get; set; }
    public string? ResearchGap { get; set; }
    public string? MainContribution { get; set; }
    public string? Rule { get; set; }
    public string? GapType { get; set; }
    public string? Journal { get; set; }
    public string? StyleName { get; set; }
    public string? StyleDescription { get; set; }
    public string? StyleRule { get; set; }
    public PaperStatus? Status { get; set; }
    public List<string>? TagNames { get; set; } = new();
    public List<Reference>? References { get; set; } = new();

    #endregion

    #region Factories

    public static PaperEntity Create(Guid id,
        string title,
        string? template = null,
        string? context = null,
        string? abstractText = null,
        string? researchGap = null,
        string? mainContribution = null,
        string? rule = null,
        string? gapType = null,
        string? journal = null,
        string? styleName = null,
        string? styleDescription = null,
        string? styleRule = null,
        PaperStatus? status = null,
        List<string>? tagNames = null,
        List<Reference>? references = null,
        string? createdBy = null)
    {
        return new PaperEntity()
        {
            Id = id,
            Title = title,
            Template = template,
            Context = context,
            Abstract = abstractText,
            ResearchGap = researchGap,
            MainContribution = mainContribution,
            Rule = rule,
            GapType = gapType,
            Journal = journal,
            StyleName = styleName,
            StyleDescription = styleDescription,
            StyleRule = styleRule,
            Status = status ?? PaperStatus.Processing,
            TagNames = tagNames ?? new(),
            References = references ?? new(),
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
            CreatedBy = createdBy
        };
    }

    #endregion

    #region Methods

    public void Update(string? title = null,
        string? template = null,
        string? context = null,
        string? abstractText = null,
        string? researchGap = null,
        string? mainContribution = null,
        string? rule = null,
        string? journal = null,
        string? styleName = null,
        string? styleDescription = null,
        string? styleRule = null,
        PaperStatus? status = null,
        string? gapType = null,
        List<string>? tagNames = null,
        List<Reference>? references = null)
    {
        Title = title ?? Title;
        Template = template ?? Template;
        Context = context ?? Context;
        Abstract = abstractText ?? Abstract;
        ResearchGap = researchGap ?? ResearchGap;
        MainContribution = mainContribution ?? MainContribution;
        Rule = rule ?? Rule;
        GapType = gapType  ?? GapType;
        Status = status ?? Status;
        TagNames = tagNames ?? TagNames;
        References = references ?? References;
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
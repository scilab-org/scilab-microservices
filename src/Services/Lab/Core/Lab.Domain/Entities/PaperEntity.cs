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
    public string? ResearchAim { get; set; }
    public string? Rule { get; set; }
    public string? GapType { get; set; }
    public string? ConferenceJournalName { get; set; }
    public Guid? ConferenceJournalId { get; set; }
    public DateTimeOffset? ConferenceJournalStartAt { get; set; }
    public DateTimeOffset? ConferenceJournalEndAt { get; set; }
    public PaperStatus? Status { get; set; }
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
        string? researchAim = null,
        string? rule = null,
        string? gapType = null,
        string? conferenceJournalName = null,
        Guid? conferenceJournalId = null,
        DateTimeOffset? conferenceJournalStartAt = null,
        DateTimeOffset? conferenceJournalEndAt = null,
        PaperStatus? status = null,
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
            ResearchAim = researchAim,
            Rule = rule,
            GapType = gapType,
            ConferenceJournalName = conferenceJournalName,
            ConferenceJournalId = conferenceJournalId,
            ConferenceJournalStartAt = conferenceJournalStartAt,
            ConferenceJournalEndAt = conferenceJournalEndAt,
            Status = status ?? PaperStatus.Processing,
            References = references ?? [],
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
        string? researchAim = null,
        string? rule = null,
        string? conferenceJournalName = null,
        Guid? conferenceJournalId = null,
        DateTimeOffset? conferenceJournalStartAt = null,
        DateTimeOffset? conferenceJournalEndAt = null,
        PaperStatus? status = null,
        string? gapType = null,
        List<Reference>? references = null,
        string? lastModifiedBy = null)
    {
        Title = title ?? Title;
        Template = template ?? Template;
        Context = context ?? Context;
        Abstract = abstractText ?? Abstract;
        ResearchGap = researchGap ?? ResearchGap;
        MainContribution = mainContribution ?? MainContribution;
        ResearchAim = researchAim ?? ResearchAim;
        Rule = rule ?? Rule;
        GapType = gapType ?? GapType;
        ConferenceJournalName = conferenceJournalName ?? ConferenceJournalName;
        ConferenceJournalId = conferenceJournalId ?? ConferenceJournalId;
        ConferenceJournalStartAt = conferenceJournalStartAt ?? ConferenceJournalStartAt;
        ConferenceJournalEndAt = conferenceJournalEndAt ?? conferenceJournalEndAt;
        Status = status ?? Status;
        References = references ?? References;
        LastModifiedBy = lastModifiedBy ?? LastModifiedBy;
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
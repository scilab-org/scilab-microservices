using Management.Domain.Abstractions;
using Management.Domain.Enums;

namespace Management.Domain.Entities;

public sealed class ProjectEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? Context { get; set; }
    public string? Domain { get; set; }
    public string? Keypoint { get; set; }
    public Guid? ParentProjectId { get; set; }
    public List<Guid> DatasetIds { get; set; } = new();
    public List<Guid> PaperIds { get; set; } = new();
    public List<Guid> ConferenceJournalIds { get; set; } = new();
    #endregion

    #region Factories

    public static ProjectEntity Create(Guid id,
        string? name = null,
        string? description = null,
        string? code = null,
        ProjectStatus? status = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null,
        Guid? parentProjectId = null,
        string? context = null,
        string? domain = null,
        string? keypoint = null,
        List<Guid>? paperIds = null,
        List<Guid>? conferenceJournalIds = null,
        string? createdBy = null)
    {
        return new ProjectEntity()
        {
            Id = id,
            Name = name,
            Description = description,
            Code = code,
            Status = status,
            StartDate = startDate,
            EndDate = endDate,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
            ParentProjectId = parentProjectId,
            Context = context,
            Domain = domain,
            Keypoint = keypoint,
            PaperIds = paperIds?.Distinct().ToList() ?? new List<Guid>(),
            ConferenceJournalIds = conferenceJournalIds?.Distinct().ToList() ?? new List<Guid>(),
            CreatedBy = createdBy
        };
    }

    #endregion

    #region Methods

    public void Update(string name,
        string? description,
        string? code,
        string? context,
        string? domain,
        string? keypoint,
        ProjectStatus? status,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate)
    {
        Name = name;
        Description = description;
        Code = code;
        Context = context;
        Domain = domain;
        Keypoint = keypoint;
        StartDate = startDate;
        Status = status;
        EndDate = endDate;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    public void AddPapers(IEnumerable<Guid> paperIds)
    {
        foreach (var paperId in paperIds.Distinct())
        {
            if (!PaperIds.Contains(paperId))
                PaperIds.Add(paperId);
        }

        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    public List<Guid> RemovePapers(IEnumerable<Guid> paperIds)
    {
        var removed = new List<Guid>();

        foreach (var id in paperIds.Distinct())
        {
            if (PaperIds.Remove(id))
                removed.Add(id);
        }

        if (removed.Any())
            LastModifiedOnUtc = DateTimeOffset.UtcNow;

        return removed;
    }

    public void AddConferenceJournals(IEnumerable<Guid> conferenceJournalIds)
    {
        foreach (var journalId in conferenceJournalIds.Distinct())
        {
            if (!ConferenceJournalIds.Contains(journalId))
                ConferenceJournalIds.Add(journalId);
        }

        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    public List<Guid> RemoveConferenceJournals(IEnumerable<Guid> conferenceJournalIds)
    {
        var removed = new List<Guid>();

        foreach (var id in conferenceJournalIds.Distinct())
        {
            if (ConferenceJournalIds.Remove(id))
                removed.Add(id);
        }

        if (removed.Any())
            LastModifiedOnUtc = DateTimeOffset.UtcNow;

        return removed;
    }
    #endregion
    
}
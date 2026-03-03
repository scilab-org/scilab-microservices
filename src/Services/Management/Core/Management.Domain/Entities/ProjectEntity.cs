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
    public Guid? ParentProjectId { get; set; }
    public List<Guid> DatasetIds { get; set; } = new();
    public List<Guid> PaperIds { get; set; } = new();
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
        List<Guid>? paperIds = null)
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
            PaperIds = paperIds?.Distinct().ToList() ?? new List<Guid>()
        };
    }

    #endregion

    #region Methods

    public void Update(string name,
        string? description,
        string? code,
        ProjectStatus? status,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate)
    {
        Name = name;
        Description = description;
        Code = code;
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
    #endregion
    
}
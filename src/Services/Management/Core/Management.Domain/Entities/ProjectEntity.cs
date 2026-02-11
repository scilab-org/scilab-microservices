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
    public Guid? PaperId { get; set; }
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
        Guid? paperId = null)
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
            PaperId = paperId
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

    #endregion
    
}
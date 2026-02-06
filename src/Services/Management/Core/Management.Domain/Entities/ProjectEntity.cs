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

    #endregion

    #region Factories

    public static ProjectEntity Create(Guid id,
        string name,
        string? description,
        string? code,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate)
    {
        return new ProjectEntity()
        {
            Id = id,
            Name = name,
            Description = description,
            Code = code,
            Status = ProjectStatus.Draft,
            StartDate = startDate,
            EndDate = endDate,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
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
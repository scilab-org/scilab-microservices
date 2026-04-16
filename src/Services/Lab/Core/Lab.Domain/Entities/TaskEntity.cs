using Lab.Domain.Abstractions;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public sealed class TaskEntity: Entity<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public TaskDefineStatus Status { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? NextReviewDate { get; set; }
    public DateTimeOffset? CompleteDate { get; set; }
    public Guid MemberId { get; set; }
    public TaskType TaskType { get; set; }
    public string? AssignedToUserName { get; set; }

    #endregion

    #region Factories

    public static TaskEntity Create(Guid id,
        Guid memberId,
        string name,
        string? description = null,
        string? assignedToUserName = null,
        TaskDefineStatus status = TaskDefineStatus.ToDo,
        TaskType taskType = TaskType.Other,
        DateTimeOffset? startDate = null,
        DateTimeOffset? nextReviewDate = null,
        DateTimeOffset? completeDate = null,
        string? createdBy = null)
    {
        return new TaskEntity()
        {
            Id = id,
            MemberId = memberId,
            Name = name,
            Description = description,
            AssignedToUserName = assignedToUserName,
            Status = status,
            TaskType = taskType,
            StartDate = startDate,
            NextReviewDate = nextReviewDate,
            CompleteDate = completeDate,
            CreatedBy = createdBy,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }

    #endregion

    #region Methods

    public void Update(string? name = null,
        string? description = null,
        Guid? memberId = null,
        TaskDefineStatus? status = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? nextReviewDate = null,
        DateTimeOffset? completeDate = null)
    {
        Name = name ?? Name;
        Description = description ?? Description;
        MemberId = memberId ?? MemberId;
        Status = status ?? Status;
        StartDate = startDate ?? StartDate;
        NextReviewDate = nextReviewDate ?? NextReviewDate;
        CompleteDate = completeDate ?? CompleteDate;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }

    #endregion
}
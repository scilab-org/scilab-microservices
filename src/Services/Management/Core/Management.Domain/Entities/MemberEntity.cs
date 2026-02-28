using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public sealed class MemberEntity : Entity<Guid>
{
    #region Fields, Properties and Indexers

    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
    
    #endregion
    
    #region Factories
    
    public static MemberEntity Create(Guid id,
        Guid userId,
        Guid projectId,
        string projectRole,
        DateTimeOffset joinedAt)
    {
        return new MemberEntity()
        {
            Id = id,
            UserId = userId,
            ProjectId = projectId,
            ProjectRole = projectRole,
            JoinedAt = joinedAt,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            LastModifiedOnUtc = DateTimeOffset.UtcNow,
        };
    }
    #endregion
    
    #region Methods
    public void UpdateProjectRole(string projectRole)
    {
        ProjectRole = projectRole;
        LastModifiedOnUtc = DateTimeOffset.UtcNow;
    }
    
    #endregion
}
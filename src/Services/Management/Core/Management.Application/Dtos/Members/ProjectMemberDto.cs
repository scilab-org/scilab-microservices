namespace Management.Application.Dtos.Members;

public sealed class ProjectMemberDto
{
    #region Fields, Properties and Indexers

    /// <summary>MemberEntity Id</summary>
    public Guid MemberId { get; set; }

    public Guid UserId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; }

    public string Role { get; set; } = default!;

    public DateTimeOffset JoinedAt { get; set; }

    #endregion
}


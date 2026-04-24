using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public sealed class ProjectMemberDto
{
    #region Fields, Properties and Indexers

    /// <summary>MemberEntity Id</summary>
    public Guid MemberId { get; set; }

    public Guid UserId { get; set; }

    /// <summary>SubProject (or Project) that this member belongs to.</summary>
    public Guid SubProjectId { get; set; }
    public Guid ProjectId { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Orcid { get; set; }

    public bool Enabled { get; set; }

    public string Role { get; set; } = default!;

    public DateTimeOffset JoinedAt { get; set; }

    #endregion
}


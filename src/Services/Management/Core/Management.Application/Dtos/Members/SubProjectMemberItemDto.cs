using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public sealed class SubProjectMemberItemDto
{
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool Enabled { get; set; }
}

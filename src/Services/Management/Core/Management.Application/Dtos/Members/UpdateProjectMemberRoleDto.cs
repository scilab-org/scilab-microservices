using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public sealed class UpdateProjectMemberRoleDto
{
    public Guid MemberId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
}


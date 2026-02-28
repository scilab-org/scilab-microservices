namespace Management.Application.Dtos.Members;

public sealed class UpdateProjectMemberRoleDto
{
    public Guid MemberId { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
}


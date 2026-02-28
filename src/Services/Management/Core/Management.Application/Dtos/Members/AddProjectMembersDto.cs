
namespace Management.Application.Dtos.Members;

public sealed class AddProjectMembersDto
{
    #region Fields, Properties and Indexers

    public List<ProjectMemberEntry> Members { get; set; } = new();

    #endregion
}

public sealed class ProjectMemberEntry
{
    #region Fields, Properties and Indexers

    public Guid UserId { get; set; }

    /// <summary>
    /// Allowed values: <see cref="AuthorizeConstants.User"/> (default) or <see cref="AuthorizeConstants.ProjectManager"/>.
    /// </summary>
    public string GroupName { get; set; } = AuthorizeConstants.User;

    #endregion
}
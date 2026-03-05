namespace Management.Application.Dtos.Members;

public class AddSubProjectMembersDto
{
    #region Fields, Properties and Indexers
    public List<ProjectMemberEntry> Members { get; set; } = new();

    #endregion
}
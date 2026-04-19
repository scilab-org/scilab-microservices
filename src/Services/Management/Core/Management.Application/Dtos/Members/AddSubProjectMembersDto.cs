using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public class AddSubProjectMembersDto
{
    #region Fields, Properties and Indexers
    public List<ProjectMemberEntry> Members { get; set; } = new();

    #endregion
}
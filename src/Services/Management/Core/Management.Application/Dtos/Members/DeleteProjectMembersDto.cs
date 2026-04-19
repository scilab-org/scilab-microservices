using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Members;

[ExcludeFromCodeCoverage]
public sealed class DeleteProjectMembersDto
{
    #region Fields, Properties and Indexers
    public List<Guid> MemberIds { get; set; } = new();
    #endregion
}

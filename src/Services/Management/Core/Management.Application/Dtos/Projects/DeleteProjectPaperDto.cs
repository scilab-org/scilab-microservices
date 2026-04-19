using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class DeleteProjectPaperDto
{
    #region Fields, Properties and Indexers
    public List<Guid> PaperIds { get; set; } = new();
    #endregion
}
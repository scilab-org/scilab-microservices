using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Dtos.Projects;

[ExcludeFromCodeCoverage]
public class CreateProjectPaperDto
{
    #region Fields, Properties and Indexers
        public List<Guid> PaperIds { get; set; } = new();
    #endregion
}
namespace Management.Application.Dtos.Projects;

public class DeleteProjectPaperDto
{
    #region Fields, Properties and Indexers
    public List<Guid> PaperIds { get; set; } = new();
    #endregion
}
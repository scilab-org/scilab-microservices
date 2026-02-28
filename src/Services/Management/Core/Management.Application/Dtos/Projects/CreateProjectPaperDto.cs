namespace Management.Application.Dtos.Projects;

public class CreateProjectPaperDto
{
    #region Fields, Properties and Indexers
        public List<Guid> PaperIds { get; set; } = new();
    #endregion
}
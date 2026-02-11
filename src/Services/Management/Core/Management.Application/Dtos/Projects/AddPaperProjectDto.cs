namespace Management.Application.Dtos.Projects;

public class AddPaperProjectDto
{
    #region Fields, Properties and Indexers
        public List<Guid> PaperIds { get; set; } = new();
    #endregion
}
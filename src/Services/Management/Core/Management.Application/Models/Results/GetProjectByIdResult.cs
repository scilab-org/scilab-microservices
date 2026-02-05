using Management.Application.Dtos.Projects;

namespace Management.Application.Models.Results;

public class GetProjectByIdResult 
{
    #region Fields, Properties and Indexers

    public ProjectDto Project { get; init; }

    #endregion
    
    #region Ctors
    public GetProjectByIdResult(ProjectDto project)
    {
        Project = project;
    }
    #endregion
}
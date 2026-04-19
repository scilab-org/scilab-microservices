using Management.Application.Dtos.Projects;

using System.Diagnostics.CodeAnalysis;

namespace Management.Application.Models.Results;

[ExcludeFromCodeCoverage]
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
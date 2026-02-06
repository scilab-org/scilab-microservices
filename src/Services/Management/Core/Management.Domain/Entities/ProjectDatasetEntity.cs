using Management.Domain.Abstractions;

namespace Management.Domain.Entities;

public class ProjectDatasetEntity :  Entity<Guid>
{
    #region Properties
    public Guid ProjectId { get; private set; }
    public Guid DatasetId { get; private set; }

    #endregion
    
    #region Ctors
    public static ProjectDatasetEntity Create(Guid projectId, Guid datasetId)
        => new()
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            DatasetId = datasetId
        };
    #endregion
}
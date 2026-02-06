using Management.Domain.Entities;

namespace Management.Infrastructure.Data;

public static class ProjectDatasetSeedData
{
    #region Methods

    public static ProjectDatasetEntity[] GetProjectDatasets()
    {
        return new[]
        {
            ProjectDatasetEntity.Create(
                ProjectSeedData.ProjectAIId,
                DatasetSeedData.CustomerDatasetId),

            ProjectDatasetEntity.Create(
                ProjectSeedData.ProjectAIId,
                DatasetSeedData.SalesDatasetId),

            ProjectDatasetEntity.Create(
                ProjectSeedData.ProjectMarketId,
                DatasetSeedData.SalesDatasetId),

            ProjectDatasetEntity.Create(
                ProjectSeedData.ProjectMarketId,
                DatasetSeedData.ResearchDatasetId)
        };
    }

    #endregion
}
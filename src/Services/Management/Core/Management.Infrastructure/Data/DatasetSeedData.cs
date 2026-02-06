using Management.Domain.Entities;

namespace Management.Infrastructure.Data;

public static class DatasetSeedData
{
    #region Constants

    public static readonly Guid CustomerDatasetId =
        Guid.Parse("b1111111-1111-1111-1111-111111111111");

    public static readonly Guid SalesDatasetId =
        Guid.Parse("b2222222-2222-2222-2222-222222222222");

    public static readonly Guid ResearchDatasetId =
        Guid.Parse("b3333333-3333-3333-3333-333333333333");

    #endregion

    #region Methods

    public static DatasetEntity[] GetDatasets(string performedBy)
    {
        return new[]
        {
            DatasetEntity.Create(
                id: CustomerDatasetId,
                name: "Customer Dataset",
                description: "Dataset about customer behavior"),

            DatasetEntity.Create(
                id: SalesDatasetId,
                name: "Sales Dataset",
                description: "Dataset about sales statistics"),

            DatasetEntity.Create(
                id: ResearchDatasetId,
                name: "Research Dataset",
                description: "Dataset for academic research")
        };
    }

    #endregion
}
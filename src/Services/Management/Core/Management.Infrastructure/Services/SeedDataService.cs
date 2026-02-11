#region using

using Common.ValueObjects;
using Management.Application.Services;
using Management.Domain.Entities;
using Management.Infrastructure.Data;
using Marten;

#endregion

namespace Management.Infrastructure.Services;

public class SeedDataService : ISeedDataService
{
    #region Implementations

    public async Task<bool> SeedDataAsync(IDocumentSession session, CancellationToken cancellationToken)
    {
        var hasChanges = false;
        var performedBy = Actor.System("lab-service").ToString();
        
        if (!await session.Query<ProjectEntity>().AnyAsync(cancellationToken))
        {
            hasChanges = true;
            var projects = ProjectSeedData.GetProjects(performedBy);
            session.Store(projects);
        }
        if (!await session.Query<DatasetEntity>().AnyAsync(cancellationToken))
        {
            hasChanges = true;
            var projects = DatasetSeedData.GetDatasets(performedBy);
            session.Store(projects);
        }
        if (hasChanges)
        {
            await session.SaveChangesAsync(cancellationToken);
        }

        return hasChanges;
    }

    #endregion

    #region Private Methods

    #endregion
}
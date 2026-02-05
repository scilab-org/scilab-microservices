#region using

using Common.ValueObjects;
using Management.Application.Services;
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
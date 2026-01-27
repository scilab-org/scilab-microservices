#region using

using Common.ValueObjects;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Lab.Infrastructure.Data;
using Marten;

#endregion

namespace Lab.Infrastructure.Services;

public sealed class SeedDataService : ISeedDataService
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


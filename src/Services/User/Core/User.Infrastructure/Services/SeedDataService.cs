#region using

using Common.ValueObjects;
using User.Application.Services;
using User.Domain.Entities;
using User.Infrastructure.Data;
using Marten;

#endregion

namespace User.Infrastructure.Services;

public sealed class SeedDataService : ISeedDataService
{
    #region Implementations

    public async Task<bool> SeedDataAsync(IDocumentSession session, CancellationToken cancellationToken)
    {
        var hasChanges = false;
        var performedBy = Actor.System("user-service").ToString();

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

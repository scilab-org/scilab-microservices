#region using

using Marten;

#endregion

namespace User.Application.Services;

public interface ISeedDataService
{
    #region Methods

    Task<bool> SeedDataAsync(IDocumentSession session, CancellationToken cancellationToken);

    #endregion
}
